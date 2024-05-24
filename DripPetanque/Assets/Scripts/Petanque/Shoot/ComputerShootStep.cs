using System;

[Serializable]
public class ComputerShootStep : BaseShootStep
{
    public override void Start()
    {
        m_stepOutputValue = (m_data.Range.x + m_data.Range.y) / 2.0f;
    }

    public override void Update(float deltaTime)
    {
    }

    public override bool IsFinished()
    {
        return true;
    }
}
