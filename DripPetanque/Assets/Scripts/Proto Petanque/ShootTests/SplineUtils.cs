using Unity.Mathematics;
using UnityEngine;

public static class SplineUtils
{
    public static Vector3 ToVector3(this float3 f3)
    {
        return new Vector3(f3.x, f3.y, f3.z);
    }
}
