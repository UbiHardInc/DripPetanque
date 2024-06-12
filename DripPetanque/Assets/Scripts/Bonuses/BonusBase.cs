using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    public virtual void OnBonusAttached(Transform ballTransform)
    {
    }

    public virtual void OnTounchGround()
    {

    }

    public virtual float OnBallStop()
    {
        return 0.0f;
    }

    public virtual void ChangeBallScore(ref int score)
    {
    }

}
