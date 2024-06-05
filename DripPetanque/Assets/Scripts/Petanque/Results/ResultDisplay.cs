using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference m_endResultInput;
    [SerializeField] private TMP_Text m_text;
    
    [SerializeField] private GameObject m_scorePanelLayout;
    [SerializeField] private GameObject m_scorePanelPrefab;

    [NonSerialized] private Action m_resultDisplayEndCallback;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void DislayRoundResult(RoundResultDatas result, Action resultDisplayEndCallback)
    {
        int roundIndex = result.RoundIndex;
        BasePetanquePlayer winner = result.Winner;
        m_text.text = $"{winner.PlayerName} won the round {roundIndex} with {winner.GetResultForRound(roundIndex).Score} points and now has {winner.CurrentScore}";

        gameObject.SetActive(true);
        CreateRoundDisplay(result.AllPlayers, result.RoundIndex, result.Winner);
        m_resultDisplayEndCallback = resultDisplayEndCallback;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    public void DisplayGameResult(GameResultDatas gameResult, Action resultDisplayEndCallback)
    {
        gameObject.SetActive(true);
        CreateRoundDisplay(gameResult.AllPlayers, gameResult.RoundCount, gameResult.Winner);
        m_text.text = $"{gameResult.Winner.PlayerName} won the game with {gameResult.Winner.CurrentScore} points";
        m_resultDisplayEndCallback = resultDisplayEndCallback;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    private void CreateRoundDisplay(List<BasePetanquePlayer> allPlayers, int roundIndex, BasePetanquePlayer winner)
    {
        BasePetanquePlayer humanPlayer = null;
        BasePetanquePlayer computerPlayer = null;

        foreach (var player in allPlayers)
        {
            switch (player.PlayerType)
            {
                case PetanquePlayerType.Human:
                    humanPlayer = player;
                    break;
                case PetanquePlayerType.Computer:
                    computerPlayer = player;
                    break;
            }
        }
        
        GameObject scorePanel = Instantiate(m_scorePanelPrefab, m_scorePanelLayout.transform);
        scorePanel.GetComponent<RoundScorePanel>().InitializeRoundScorePanel(roundIndex, humanPlayer.CurrentScore,
            computerPlayer.CurrentScore, winner.PlayerType);
    }

    private void OnEndResultInputPerformed(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        m_endResultInput.action.performed -= OnEndResultInputPerformed;
        m_resultDisplayEndCallback();
    }

}
