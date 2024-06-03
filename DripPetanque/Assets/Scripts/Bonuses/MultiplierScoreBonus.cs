using UnityEngine;

public class MultiplierScoreBonus : BonusBase
{
    [SerializeField] private int m_scoreMultiplier;

    public override void ChangeBallScore(ref int score)
    {
        base.ChangeBallScore(ref score);
        score *= m_scoreMultiplier;
    }
}
