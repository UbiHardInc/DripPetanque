using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTests : MonoBehaviour
{
    private enum InterpolationFunctionEnum
    {
        Smoothstep,
        Smootherstep,
    }

    [SerializeField] private SplineContainer m_spline;
    [SerializeField] private float m_speed = 0.5f;
    [SerializeField] private InterpolationFunctionEnum m_interpolationFunction;

    [NonSerialized] private Transform m_transform;

    private void Start()
    {
        m_transform = transform;
    }

    private void Update()
    {
        float t = MathF.Abs((Time.time * m_speed) % 2 - 1);
        float3 pos = m_spline.EvaluatePosition(InterpolationFunction(t));
        m_transform.position = pos;
    }

    private float InterpolationFunction(float t)
    {
        return m_interpolationFunction switch
        {
            InterpolationFunctionEnum.Smoothstep => Smoothstep(t),
            InterpolationFunctionEnum.Smootherstep => Smootherstep(t),
            _ => throw new NotImplementedException(),
        };
    }

    float Smoothstep(float w)
    {
        return w * w * (3.0f - 2.0f * w);
    }

    float Smootherstep(float w)
    {
        return ((w * (w * 6.0f - 15.0f) + 10.0f) * w * w * w);
    }
}
