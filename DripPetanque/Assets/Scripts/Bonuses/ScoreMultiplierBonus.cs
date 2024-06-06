using UnityEngine;

public class ScoreMultiplierBonus : BonusBase
{
    [SerializeField] private int m_scoreMultiplier;

    public override void ChangeBallScore(ref int score)
    {
        base.ChangeBallScore(ref score);
        score *= m_scoreMultiplier;
    }
}
