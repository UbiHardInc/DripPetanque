using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class CustomSplineController : MonoBehaviour
{
    [SerializeField] private Transform m_startPoint = null;
    [SerializeField] private Transform m_endPoint = null;

    [SerializeField] private float m_angle = 0.0f;
    [SerializeField] private float m_height = 0.0f;
    [SerializeField, Range(0.0001f, 0.9999f)] private float m_forwardFactor = 0.5f;
    [SerializeField, Range(0.0f, 2.0f)] private float m_attractionFactor = 0.0f;



    [SerializeField] private SplineContainer m_splineComponent = null;

    [NonSerialized] private Spline m_spline = null;

    [NonSerialized] private BezierKnot m_firstKnot;
    [NonSerialized] private BezierKnot m_lastKnot;


    // Gizmos
    [NonSerialized] private Vector3 m_attractionPointPosition = Vector3.zero;

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
        UpdateSpline();
    }

    private void UpdateSpline()
    {
        Vector3 startPosition = m_startPoint.position;
        Vector3 endPosition = m_endPoint.position;

        Vector3 startToEnd = endPosition - startPosition;
        Vector3 attractionPointProj = startPosition + startToEnd * m_forwardFactor;

        Vector3 splineUp = Vector3.up * Mathf.Cos(m_angle * Mathf.Deg2Rad) +
                           Vector3.Cross(startToEnd, Vector3.up).normalized * Mathf.Sin(m_angle * Mathf.Deg2Rad);

        Vector3 attractionPoint = attractionPointProj + splineUp * m_height;
        m_attractionPointPosition = attractionPoint;

        m_firstKnot.Position = startPosition;
        m_firstKnot.TangentOut = (attractionPoint - startPosition) * m_attractionFactor;

        m_lastKnot.Position = endPosition;
        m_lastKnot.TangentOut = (attractionPoint - endPosition) * -m_attractionFactor;


        m_spline.SetKnot(0, m_firstKnot);
        m_spline.SetKnot(1, m_lastKnot);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_attractionPointPosition, 0.3f);
        }
    }
#endif
}
