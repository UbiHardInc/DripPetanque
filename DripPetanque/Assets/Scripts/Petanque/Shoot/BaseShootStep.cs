using System;
using UnityEngine;

[Serializable]
public abstract class BaseShootStep
{
    public float StepOutputValue => m_stepOutputValue;

    [SerializeField] protected ShootStepData m_data;

    [NonSerialized] protected float m_stepOutputValue;

    public abstract void Start();
    public abstract void Update(float deltaTime);
    public abstract bool IsFinished();

    public virtual void Dispose()
    {

    }

}
