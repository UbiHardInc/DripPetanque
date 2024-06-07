using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    public virtual void OnBonusAttached(Transform ballTransform)
    {
    }

    public virtual void OnTounchGround()
    {

    }

    public virtual void OnBallStop()
    {

    }

    public virtual void ChangeBallScore(ref int score)
    {
    }

}
