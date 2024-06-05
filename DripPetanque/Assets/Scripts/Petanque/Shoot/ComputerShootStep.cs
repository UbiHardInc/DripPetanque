using System;
using UnityEngine;

[Serializable]
public class ComputerShootStep : BaseShootStep
{
    [SerializeField, Min(0.0f)] private float m_noiseAmount = 0.0f;

    [NonSerialized] private float m_givenValue = float.NaN;

    public override void Start()
    {
        m_stepOutputValue = m_givenValue;
    }

    public override void Update(float deltaTime)
    {
    }

    public override bool IsFinished()
    {
        return !float.IsNaN(m_stepOutputValue);
    }

    public override bool HasTempValue()
    {
        return false;
    }

    public override void Dispose()
    {
        base.Dispose();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        m_stepOutputValue = float.NaN;
        m_givenValue = float.NaN;
    }

    public void SetValue(float value)
    {
        float noise = UnityEngine.Random.value * m_noiseAmount;
        float noisedValue = Mathf.Clamp(value + noise, Range.x, Range.y);
        m_givenValue = noisedValue;
        m_stepOutputValue = noisedValue;
    }

    public void SetRandomValue()
    {
        m_givenValue = Mathf.Lerp(Range.x, Range.y, UnityEngine.Random.value);
        m_stepOutputValue = m_givenValue;
    }
}
