using UnityEngine;

public class MultiplierScoreBonus : BonusBase
{
    [SerializeField] private int m_scoreMultiplier;

    public override int ChangeBallScore(int score)
    {
        int modifiedScore = score * m_scoreMultiplier;
        return base.ChangeBallScore(modifiedScore);
    }
}
