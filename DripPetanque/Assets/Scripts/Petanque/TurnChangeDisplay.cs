using System.Collections;
using TMPro;
using UnityEngine;

public class TurnChangeDisplay : MonoBehaviour
{
    [SerializeField] private float m_secondsOfFades = 1.0f;
    [SerializeField] private float m_secondsOfDisplay = 2.0f;

    [SerializeField] private TMP_Text m_text;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator DisplayTurn(bool isPlayer)
    {
        m_text.text = isPlayer ? "Your Turn" : "Opponent's Turn";
        yield return FadeInAndOutGameObject.FadeInAndOut(gameObject, true, m_secondsOfFades);
        m_text.gameObject.SetActive(true);

        yield return new WaitForSeconds(m_secondsOfDisplay);

        m_text.gameObject.SetActive(false);
        yield return FadeInAndOutGameObject.FadeInAndOut(gameObject, false, m_secondsOfFades);
    }
}
