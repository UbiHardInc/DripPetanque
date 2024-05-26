using System;
using UnityUtility.Utils;

[Serializable]
public class ComputerShootStep : BaseShootStep
{
    public override void Start()
    {
        m_stepOutputValue = UnityEngine.Random.value.RemapFrom01(m_data.Range);
    }

    public override void Update(float deltaTime)
    {
    }

    public override bool IsFinished()
    {
        return true;
    }

    public override bool HasTempValue()
    {
        return false;
    }
}
