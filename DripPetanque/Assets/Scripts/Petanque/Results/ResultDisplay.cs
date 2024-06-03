using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference m_endResultInput;
    [SerializeField] private TMP_Text m_text;

    [NonSerialized] private Action m_resultDisplayEndCallback;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void DislayGameResult(GameResultDatas result, Action resultDisplayEndCallback)
    {
        gameObject.SetActive(true);
        m_text.text = $"{result.Winner.PlayerName} won the game with {result.Winner.CurrentScore} points";
        m_resultDisplayEndCallback = resultDisplayEndCallback;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    public void DislayRoundResult(RoundResultDatas result, Action resultDisplayEndCallback)
    {
        int roundIndex = result.RoundIndex;
        BasePetanquePlayer winner = result.Winner;
        m_text.text = $"{winner.PlayerName} won the round {roundIndex} with {winner.GetResultForRound(roundIndex).Score} points and now has {winner.CurrentScore}";

        gameObject.SetActive(true);
        m_resultDisplayEndCallback = resultDisplayEndCallback;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    private void OnEndResultInputPerformed(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        m_endResultInput.action.performed -= OnEndResultInputPerformed;
        m_resultDisplayEndCallback();
    }

}
