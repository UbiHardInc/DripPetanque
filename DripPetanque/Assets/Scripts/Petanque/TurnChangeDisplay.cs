using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnChangeDisplay : MonoBehaviour
{
    [SerializeField] private float m_secondsOfFades = 1;
    
    [SerializeField] private TMP_Text m_text;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator DisplayTurn(bool isPlayer)
    {
        StartCoroutine(FadeInAndOutGameObject.FadeInAndOut(gameObject, true, m_secondsOfFades));
        yield return new WaitForSeconds(m_secondsOfFades);
        m_text.gameObject.SetActive(true);
        m_text.text = isPlayer ? "Your Turn" : "Opponent's Turn";
    }

    public void CloseTurnPanel()
    {
        m_text.gameObject.SetActive(false);
        StartCoroutine(FadeInAndOutGameObject.FadeInAndOut(gameObject, false, m_secondsOfFades));
        
    }
}
