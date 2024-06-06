using UnityEngine;

public class AddToScoreBonus : BonusBase
{
    [SerializeField] private int m_scoeToAdd;

    public override void ChangeBallScore(ref int score)
    {
        base.ChangeBallScore(ref score);
        score += m_scoeToAdd;
    }
}
