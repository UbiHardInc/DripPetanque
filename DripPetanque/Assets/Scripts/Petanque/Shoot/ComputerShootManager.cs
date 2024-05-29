using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Utils;

public class ComputerShootManager : BaseShootManager<ComputerShootStep, Ball>
{
    protected override PetanqueSubGameManager.PetanquePlayers Owner => PetanqueSubGameManager.PetanquePlayers.Computer;

    [Title("Target Zone")]
    [SerializeField] private float m_targetZoneCenterDistance;
    [SerializeField] private Vector2 m_targetZoneSize;

    // The code in this class may not be really clear but don't worry, I did the maths
    private void ComputeShootDatas(Vector3 m)
    {
        Vector3 startPosition = Vector3.zero;

        float attractionFactor = m_splineController.AttractionFactor;


        Vector3 forward = transform.forward;
        Vector3 splineUp = transform.up;

        Vector3 mPrime = Vector3.ProjectOnPlane(m - startPosition, splineUp);
        Vector3 splineForward = mPrime.normalized;

        float yAngle = Vector3.Angle(forward, splineForward);

        float force = ComputeForce(yAngle);

        // t Computation
        float mx = mPrime.magnitude;
        float sx = 0;
        float cx = force * m_splineController.ForceMultiplier;
        float ex = 2 * cx;

        float spx = Mathf.Lerp(sx, cx, attractionFactor);
        float epx = Mathf.Lerp(ex, cx, attractionFactor);

        float t = ComputeT(sx, spx, epx, ex, mx);

        // attraction point computation
        float my = (m - mPrime).magnitude;
        float floorHeight = 0; // = sy = ey

        float attractionPointHeight = ComputeAttractionPointHeight(my, floorHeight, t, attractionFactor);
        float xAngle = Mathf.Atan(attractionPointHeight / cx);

        // All of that to compute yAngle, xAngle and force
    }

    private float ComputeForce(float yAngle)
    {
        float leftBound = m_targetZoneCenterDistance - m_targetZoneSize.x / 2;
        float rightBound = m_targetZoneCenterDistance + m_targetZoneSize.x / 2;
        float bottomBound = m_targetZoneCenterDistance - m_targetZoneSize.y / 2;
        float topBound = m_targetZoneCenterDistance + m_targetZoneSize.y / 2;

        float dr1, dr2;
        if (yAngle == 0.0f)
        {
            dr1 = bottomBound;
            dr2 = topBound;
        }
        else
        {
            float r1x, r2x;
            float tanAngle = Mathf.Tan(yAngle);

            // The equation of the line from s to m
            float D(float x)
            {
                return x / tanAngle;
            }

            r1x = bottomBound * tanAngle;

            if (!r1x.Between(leftBound, rightBound))
            {
                Debug.LogError("Unable to compute force to target this point");
                return 0.0f;
            }

            r2x = Mathf.Clamp(topBound * tanAngle, leftBound, rightBound);

            dr1 = Mathf.Sqrt(r1x * r1x + D(r1x) * D(r1x));
            dr2 = Mathf.Sqrt(r2x * r2x + D(r2x) * D(r2x));
        }

        dr1 /= 2 * m_splineController.ForceMultiplier;
        dr2 /= 2 * m_splineController.ForceMultiplier;

        Vector2 forceRange = m_forceStep.Range;

        dr1 = Mathf.Clamp(dr1, forceRange.x, forceRange.y);
        dr2 = Mathf.Clamp(dr2, forceRange.x, forceRange.y);

        return Mathf.Lerp(dr1, dr2, UnityEngine.Random.value);
    }

    /// <summary>
    /// Computes the time at which the point m lies
    /// </summary>
    /// <param name="s">The x coordinates of the start point</param>
    /// <param name="sTan">The x coordinates of the tangent of the start point</param>
    /// <param name="eTan">The x coordinates of the tangent of the end point</param>
    /// <param name="e">The x coordinates of the end point</param>
    /// <param name="m">The x coordinates of the m point</param>
    /// <returns></returns>
    private float ComputeT(float s, float sTan, float eTan, float e, float m)
    {
        float a = e - 3 * eTan + 3 * sTan - s;
        float b = 3 * eTan - 6 * sTan + 3 * s;
        float c = 3 * sTan - 3 * s;
        float d = s - m;

        (double r0, double r1, double r2) = CubicRootsFinder.RealRoots(a, b, c, d);

        Span<double> allRoots = stackalloc double[]
        {
            r0,
            r1,
            r2,
        };

        // Among the 3 roots of the polynomial, one of them should be a real number between 0 and 1.
        // This is the t value we are looking for
        foreach (double root in allRoots)
        {
            if (!double.IsNaN(root) && root.Between(0, 1))
            {
                return (float)root;
            }
        }
        Debug.LogError("Not able to find a valid root");
        return 0;
    }

    private float ComputeAttractionPointHeight(float my, float floorHeight, float t, float attractionFactor)
    {
        float numerator = my - floorHeight * (-t * t * t + 4 * t * t - 3 * t + 1);
        float denominator = -3 * t * t + 3 * t;

        float ceillingHeight = numerator / denominator;
        return ceillingHeight / attractionFactor;
    }

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position + transform.forward * m_targetZoneCenterDistance;

        Vector3 plb = center + new Vector3(-m_targetZoneSize.x / 2, 0, -m_targetZoneSize.y / 2);
        Vector3 plt = center + new Vector3(-m_targetZoneSize.x / 2, 0, m_targetZoneSize.y / 2);
        Vector3 prb = center + new Vector3(m_targetZoneSize.x / 2, 0, -m_targetZoneSize.y / 2);
        Vector3 prt = center + new Vector3(m_targetZoneSize.x / 2, 0, m_targetZoneSize.y / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(plb, plt);
        Gizmos.DrawLine(prb, prt);
        Gizmos.DrawLine(plb, prb);
        Gizmos.DrawLine(plt, prt);
    }

    private Vector3 X0Y(Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

}
