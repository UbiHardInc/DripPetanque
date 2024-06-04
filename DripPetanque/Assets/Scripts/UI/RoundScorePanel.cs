using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundScorePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text m_roundNumber;
    [SerializeField] private TMP_Text m_humanScore;
    [SerializeField] private TMP_Text m_computerScore;
    
    [SerializeField] private Color m_colorWinText;

    public void InitializeRoundScorePanel(int roundNumber, int humanScore, int computerScore, PetanquePlayerType winner)
    {
        m_roundNumber.text = roundNumber.ToString();
        m_humanScore.text = humanScore.ToString();
        m_computerScore.text = computerScore.ToString();

        switch (winner)
        {
            case PetanquePlayerType.Human:
                m_humanScore.color = m_colorWinText;
                break;
            case PetanquePlayerType.Computer:
                m_humanScore.color = m_colorWinText;
                break;
        }
    }
}
