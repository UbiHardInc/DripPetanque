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
        if (winner.PlayerType == PetanquePlayerType.Human)
        {
            SoundManager.Instance.PlayUISFX("applaud");
        }
        CreateRoundDisplay(result.AllPlayers, result.RoundIndex, result.Winner);
        m_resultDisplayEndCallback = resultDisplayEndCallback;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    public void DisplayGameResult(GameResultDatas gameResult, Action resultDisplayEndCallback)
    {
        gameObject.SetActive(true);
        CreateRoundDisplay(gameResult.AllPlayers, gameResult.RoundCount, gameResult.Winner);
        if (gameResult.Winner.PlayerType == PetanquePlayerType.Human)
        {
            SoundManager.Instance.PlayUISFX("applaud");
        }
        m_text.text = $"{gameResult.Winner.PlayerName} won the game with {gameResult.Winner.CurrentScore} points";
        m_resultDisplayEndCallback = resultDisplayEndCallback;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    private void CreateRoundDisplay(List<BasePetanquePlayer> allPlayers, int roundIndex, BasePetanquePlayer winner)
    {
        Span<int> scores = stackalloc int[allPlayers.Count];
        int winnerIndex = -1;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            BasePetanquePlayer player = allPlayers[i];
            if (player == winner)
            {
                winnerIndex = i;
            }

            scores[i] = player.CurrentScore;
        }

        GameObject scorePanel = Instantiate(m_scorePanelPrefab, m_scorePanelLayout.transform);

        scorePanel.GetComponent<RoundScorePanel>().InitializeRoundScorePanel(roundIndex, scores, winnerIndex);
    }

    private void OnEndResultInputPerformed(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        m_endResultInput.action.performed -= OnEndResultInputPerformed;
        m_resultDisplayEndCallback();
    }

}
