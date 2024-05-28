using System;
using UnityEngine;
using UnityUtility.Utils;

public class ComputerShootManager : BaseShootManager<ComputerShootStep, Ball>
{
    protected override PetanqueSubGameManager.PetanquePlayers Owner => PetanqueSubGameManager.PetanquePlayers.Computer;

    [SerializeField] private Rect m_targetZone;

    private void ComputeShootDatas(Vector3 m)
    {
        Vector3 startPosition = Vector3.zero;

        float attractionFactor = 0.5f;
        float forcePow = 0.5f;

        float force = 2; //@TODO : Compute force from targetted zone

        Vector3 forward = transform.forward;
        Vector3 splineUp = transform.up;

        Vector3 mPrime = Vector3.ProjectOnPlane(m - startPosition, splineUp);
        Vector3 splineForward = mPrime.normalized;

        float yAngle = Vector3.Angle(forward, splineForward);

        // t Computation
        float mx = mPrime.magnitude;
        float sx = 0;
        float cx = Mathf.Pow(force, forcePow);
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

    private float ComputeT(float s, float sp, float ep, float e, float m)
    {
        float a = e - 3 * ep + 3 * sp - s;
        float b = 3 * ep - 6 * sp + 3 * s;
        float c = 3 * sp - 3 * s;
        float d = s - m;

        (double r0, double r1, double r2) = CubicRootsFinder.RealRoots(a, b, c, d);

        Span<double> allRoots = stackalloc double[]
        {
            r0,
            r1,
            r2,
        };

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
        Vector3 center = transform.position + X0Y(m_targetZone.position);
        Vector3 p1 = center + new Vector3();
    }

    private Vector3 X0Y(Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

}
