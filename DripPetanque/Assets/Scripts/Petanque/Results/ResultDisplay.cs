using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference m_endResultInput;
    [SerializeField] private TMP_Text m_text;

    [NonSerialized] private Action m_onDisplayResultEnds;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void DislayResult(ResultDatas result, Action onDisplayResultEnds)
    {
        gameObject.SetActive(true);
        m_text.text = $"{result.Winner} won the game with {result.Points} points";
        m_onDisplayResultEnds = onDisplayResultEnds;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    private void OnEndResultInputPerformed(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        m_endResultInput.action.performed -= OnEndResultInputPerformed;
        m_onDisplayResultEnds();
    }
}
