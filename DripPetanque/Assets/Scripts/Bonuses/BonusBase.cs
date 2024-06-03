using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    [SerializeField] private Sprite m_bonusSprite;

    public Sprite BonusSprite => m_bonusSprite;

    public virtual void OnBonusAttached()
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
