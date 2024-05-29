using System;
using UnityEngine;
using UnityUtility.Utils;

[Serializable]
public abstract class BaseShootStep
{
    public float StepOutputValue => HasTempValue() || IsFinished() ? m_stepOutputValue : 0.5f.RemapFrom01(m_data.Range);
    public Vector2 Range => m_data.Range;

    [SerializeField] protected ShootStepData m_data;

    [NonSerialized] protected float m_stepOutputValue;
    [NonSerialized] protected bool m_finished;

    public abstract void Start();
    public abstract void Update(float deltaTime);
    public abstract bool IsFinished();
    public abstract bool HasTempValue();

    public virtual void Dispose()
    {

    }

}
