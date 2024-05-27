using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.CustomAttributes;
using UnityUtility.Utils;

public class CustomSplineController : MonoBehaviour
{
    [SerializeField] private Transform m_startPoint = null;
    [SerializeField] private Transform m_endPoint = null;

    [SerializeField] private float m_angle = 0.0f;
    [SerializeField] private float m_height = 0.0f;
    [SerializeField, Range(0.0001f, 0.9999f)] private float m_forwardFactor = 0.5f;
    [SerializeField, Range(0.0f, 2.0f)] private float m_attractionFactor = 0.0f;

    [SerializeField, Min(0.0f)] private float m_forcePow = 1.0f;
    [SerializeField, Min(0.0f)] private float m_forceMult = 1.0f;

    [SerializeField] private SplineContainer m_splineComponent = null;

    [NonSerialized] private Spline m_spline = null;

    [NonSerialized] private BezierKnot m_firstKnot;
    [NonSerialized] private BezierKnot m_lastKnot;


    // Gizmos
    [NonSerialized] private Vector3 m_attractionPointPosition = Vector3.zero;
    [NonSerialized] private Vector3 m_tangentStart = Vector3.zero;
    [NonSerialized] private Vector3 m_tangentEnd = Vector3.zero;

    [ContextMenu(nameof(Start))]
    private void Start()
    {
        m_spline = m_splineComponent.Spline;

        m_firstKnot = new BezierKnot(m_startPoint.position, float3.zero, float3.zero, quaternion.identity);
        m_lastKnot = new BezierKnot(m_endPoint.position, float3.zero, float3.zero, quaternion.identity);
        m_spline.Knots = new BezierKnot[2] {m_firstKnot, m_lastKnot};

        m_spline.SetTangentMode(TangentMode.Mirrored);
    }

    private void Update()
    {
        //UpdateSpline();
    }

    private void OldMethod()
    {

        Vector3 startPosition = m_startPoint.position;
        Vector3 endPosition = m_endPoint.position;

        Vector3 startToEnd = endPosition - startPosition;
        Vector3 attractionPointProj = startPosition + startToEnd * m_forwardFactor;

        Vector3 splineUp = Vector3.up * Mathf.Cos(m_angle * Mathf.Deg2Rad) +
                           Vector3.Cross(startToEnd, Vector3.up).normalized * Mathf.Sin(m_angle * Mathf.Deg2Rad);

        Vector3 attractionPoint = attractionPointProj + splineUp * m_height;
        UpdateSpline(attractionPoint);
    }

    private void UpdateSpline(Vector3 attractionPoint)
    {
        Vector3 startPosition = m_startPoint.position;
        Vector3 endPosition = m_endPoint.position;

        m_attractionPointPosition = attractionPoint;

        m_firstKnot.Position = startPosition;
        m_tangentStart = (attractionPoint - startPosition) * m_attractionFactor;
        m_firstKnot.TangentOut = m_tangentStart;

        m_lastKnot.Position = endPosition;
        m_tangentEnd = (attractionPoint - endPosition) * -m_attractionFactor;
        m_lastKnot.TangentOut = m_tangentEnd;


        m_spline.SetKnot(0, m_firstKnot);
        m_spline.SetKnot(1, m_lastKnot);
    }

    public void SetSplineParameters(float yAngle, float xAngle, float force)
    {
        float forceMagnitude = Mathf.Pow(force, m_forcePow) * m_forceMult;
        Vector3 splineForward = transform.forward.Rotate(transform.up, yAngle);
        Vector3 attractionPoint = splineForward.Rotate(-Vector3.Cross(splineForward, transform.up), xAngle).normalized * forceMagnitude;
        attractionPoint = m_startPoint.position + attractionPoint;

        m_endPoint.position = attractionPoint.ProjectOn(splineForward) / m_forwardFactor;

        UpdateSpline(attractionPoint);
    }

#if UNITY_EDITOR

    [Title("Debug")]
    [SerializeField] private float m_yAngle;
    [SerializeField] private float m_xAngle;
    [SerializeField] private float m_force;

    [ContextMenu(nameof(UpdateSplineParameters))]
    private void UpdateSplineParameters()
    {
        SetSplineParameters(m_yAngle, m_xAngle, m_force);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_attractionPointPosition, 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_firstKnot.Position, m_firstKnot.Position.ToVector3() + m_tangentStart);
            Gizmos.DrawLine(m_lastKnot.Position, m_lastKnot.Position.ToVector3() - m_tangentEnd);
        }
    }
#endif
}
