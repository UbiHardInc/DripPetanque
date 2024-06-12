using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.SerializedDictionary;
using UnityUtility.Utils;

public class ResultDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference m_endResultInput;
    [SerializeField] private TMP_Text m_text;

    [SerializeField] private GameObject m_scorePanelLayout;
    [SerializeField] private SerializedDictionary<int, GameObject> m_scorePanelPrefabs;

    [NonSerialized] private Action m_resultDisplayEndCallback;
    [NonSerialized] private List<RoundScorePanel> m_scorePanels;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void ResetPanel()
    {
        m_scorePanels.ForEach(panel => panel.gameObject.Destroy());
        m_scorePanels.Clear();
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
        int playersCount = allPlayers.Count;
        Span<int> scores = stackalloc int[playersCount];
        int winnerIndex = -1;

        for (int i = 0; i < playersCount; i++)
        {
            BasePetanquePlayer player = allPlayers[i];
            if (player == winner)
            {
                winnerIndex = i;
            }

            scores[i] = player.CurrentScore;
        }

        if (m_scorePanelPrefabs.TryGetValue(playersCount, out GameObject scorePanelPrefab))
        {
            GameObject scorePanel = Instantiate(scorePanelPrefab, m_scorePanelLayout.transform);

            RoundScorePanel roundScorePanel = scorePanel.GetComponent<RoundScorePanel>();
            m_scorePanels.Add(roundScorePanel);
            roundScorePanel.InitializeRoundScorePanel(roundIndex, scores, winnerIndex);
            return;
        }
        Debug.LogError($"No prefab to diplay the score of {playersCount} players", this);
    }

    private void OnEndResultInputPerformed(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        m_endResultInput.action.performed -= OnEndResultInputPerformed;
        m_resultDisplayEndCallback();
    }

}
