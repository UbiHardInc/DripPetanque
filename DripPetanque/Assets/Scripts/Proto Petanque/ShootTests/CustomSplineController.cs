using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.CustomAttributes;
using UnityUtility.Utils;

public class CustomSplineController : MonoBehaviour
{
    public float ForceMultiplier => m_forceMultiplier;
    public float AttractionFactor => m_attractionFactor;

    [SerializeField, Range(0.0f, 2.0f)] private float m_attractionFactor = 0.0f;

    [SerializeField, Min(0.0f)] private float m_forceMultiplier = 1.0f;

    [SerializeField] private SplineContainer m_splineComponent = null;

    [NonSerialized] private Spline m_spline = null;

    [NonSerialized] private BezierKnot m_firstKnot;
    [NonSerialized] private BezierKnot m_lastKnot;

    [NonSerialized] private bool m_init = false;


    // Gizmos
    [NonSerialized] private Vector3 m_attractionPointPosition = Vector3.zero;
    [NonSerialized] private Vector3 m_tangentStart = Vector3.zero;
    [NonSerialized] private Vector3 m_tangentEnd = Vector3.zero;

    [ContextMenu(nameof(Start))]
    private void Start()
    {
        InitIfNeeded();
    }

    private void InitIfNeeded()
    {
        if (!m_init)
        {
            m_spline = m_splineComponent.Spline;

            m_firstKnot = new BezierKnot(transform.localPosition, float3.zero, float3.zero, quaternion.identity);
            m_lastKnot = new BezierKnot(transform.localPosition, float3.zero, float3.zero, quaternion.identity);
            m_spline.Knots = new BezierKnot[2] { m_firstKnot, m_lastKnot };

            m_spline.SetTangentMode(TangentMode.Mirrored);

            m_init = true;
        }
    }

    private void UpdateSpline(Vector3 startPoint, Vector3 endPoint, Vector3 attractionPoint)
    {
        InitIfNeeded();


        m_attractionPointPosition = attractionPoint;

        m_firstKnot.Position = startPoint;
        m_tangentStart = (attractionPoint - startPoint) * m_attractionFactor;
        m_firstKnot.TangentOut = m_tangentStart;

        m_lastKnot.Position = endPoint;
        m_tangentEnd = (attractionPoint - endPoint) * -m_attractionFactor;
        m_lastKnot.TangentOut = m_tangentEnd;


        m_spline.SetKnot(0, m_firstKnot);
        m_spline.SetKnot(1, m_lastKnot);
        m_spline.SetTangentMode(TangentMode.Mirrored);
    }

    public void SetSplineParameters(SplineDatas splineDatas)
    {
        SetSplineParameters(splineDatas.YAngle, splineDatas.XAngle, splineDatas.Force);
    }

    public void SetSplineParameters(float yAngle, float xAngle, float force)
    {
        Vector3 startPoint = Vector3.zero;

        float forceMagnitude = force * m_forceMultiplier;

        Vector3 splineUp = transform.up;
        Vector3 splineForward = transform.forward.Rotate(splineUp, yAngle);

        Vector3 projectedAttractionPoint = splineForward * forceMagnitude;

        Vector3 attractionPoint = projectedAttractionPoint + splineUp * Mathf.Tan(xAngle * Mathf.Deg2Rad) * forceMagnitude;
        attractionPoint += startPoint;

        Vector3 endPoint = startPoint + projectedAttractionPoint * 2;

        UpdateSpline(startPoint, endPoint, attractionPoint);
    }

#if UNITY_EDITOR

    [Title("Debug")]
    [SerializeField] private float m_yAngle;
    [SerializeField] private float m_xAngle;
    [SerializeField] private float m_force;

    [SerializeField, Min(1)] private int m_resolution;

    [ContextMenu(nameof(UpdateSplineParameters))]
    private void UpdateSplineParameters()
    {
        SetSplineParameters(m_yAngle, m_xAngle, m_force);
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_attractionPointPosition, 0.3f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(position + m_firstKnot.Position.ToVector3(), position + m_firstKnot.Position.ToVector3() + m_tangentStart);
        Gizmos.DrawLine(position + m_lastKnot.Position.ToVector3(), position + m_lastKnot.Position.ToVector3() - m_tangentEnd);

        Span<Vector3> splinePoints = stackalloc Vector3[4]
        {
            position + m_firstKnot.Position.ToVector3(),
            position + m_firstKnot.Position.ToVector3() + m_tangentStart,
            position + m_lastKnot.Position.ToVector3() - m_tangentEnd,
            position + m_lastKnot.Position.ToVector3(),
        };

        DrawBezier(splinePoints, m_resolution);
    }

    private void DrawBezier(Span<Vector3> points, int resolution)
    {
        for (int i = 0; i < resolution - 1; i++)
        {
            Gizmos.DrawLine(GetSplineValue(points, (float)i / resolution), GetSplineValue(points, ((float)(i + 1)) / resolution));
        }
    }

    private Vector3 GetSplineValue(Span<Vector3> points, float t)
    {
        int pointsCount = points.Length;

        if (pointsCount == 2)
        {
            return Vector3.Lerp(points[0], points[1], t);
        }

        Span<Vector3> interPoints = stackalloc Vector3[pointsCount - 1];
        for (int i = 0; i < pointsCount - 1; i++)
        {
            interPoints[i] = Vector3.Lerp(points[i], points[i + 1], t);
        }
        return GetSplineValue(interPoints, t);
    }

#endif
}
