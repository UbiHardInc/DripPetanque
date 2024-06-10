using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_playerName;
    [SerializeField] private TMP_Text m_computerName;
    [SerializeField] private TMP_Text m_playerScore;
    [SerializeField] private TMP_Text m_computerScore;

    public void ActivateScoreDisplay(List<BasePetanquePlayer> AllPlayers)
    {
        this.gameObject.SetActive(true);
        foreach (var player in AllPlayers)
        {
            if (player.PlayerType == PetanquePlayerType.Human)
            {
                m_playerName.text = player.PlayerName;
            }
            else
            {
                m_computerName.text = player.PlayerName;
            }
        }
    }

    public void UpdateScore(RoundResultDatas result)
    {
        switch (result.Winner.PlayerType)
        {
            case PetanquePlayerType.Human:
                m_playerScore.text = result.Winner.CurrentScore.ToString();
                break;
            case PetanquePlayerType.Computer:
                m_computerScore.text = result.Winner.CurrentScore.ToString();
                break;
            default:
                break;
        }
    }

    public void DeactivateScoreUI()
    {
        this.gameObject.SetActive(false);
    }
}
