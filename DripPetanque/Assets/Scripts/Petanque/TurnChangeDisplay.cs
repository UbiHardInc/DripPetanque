using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnChangeDisplay : MonoBehaviour
{
    [SerializeField] private float m_secondsOfFades = 1.0f;
    [SerializeField] private float m_secondsOfDisplay = 2.0f;

    [SerializeField] private TMP_Text m_text;

    [NonSerialized] private bool m_coroutineRunning = false;
    [NonSerialized]
    private readonly Dictionary<PetanquePlayerType, string> m_changeTurnMessages = new Dictionary<PetanquePlayerType, string>()
    {
        { PetanquePlayerType.Human, "Your Turn" },
        { PetanquePlayerType.Computer, "Opponent's Turn" },
    };

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void DisplayTurn(BasePetanquePlayer player, Action callback)
    {
        if (m_coroutineRunning)
        {
            Debug.LogError("A coroutine is already running");
            return;
        }
        _ = StartCoroutine(DisplayTurnCoroutine(player, callback));
    }

    private IEnumerator DisplayTurnCoroutine(BasePetanquePlayer player, Action callback)
    {
        m_coroutineRunning = true;

        m_text.text = m_changeTurnMessages[player.PlayerType];
        yield return FadeInAndOutGameObject.FadeInAndOut(gameObject, true, m_secondsOfFades);
        m_text.gameObject.SetActive(true);

        yield return new WaitForSeconds(m_secondsOfDisplay);

        m_text.gameObject.SetActive(false);
        yield return FadeInAndOutGameObject.FadeInAndOut(gameObject, false, m_secondsOfFades);

        m_coroutineRunning = false;

        callback();
    }
}
