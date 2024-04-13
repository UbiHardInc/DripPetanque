using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    public virtual void OnTounchGround()
    {

    }

    public virtual void OnBallStop()
    {

    }

    public virtual int ChangeBallScore(int score)
    {
        return score;
    }
}
