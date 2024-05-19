using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference m_endResultInput;

    [NonSerialized] private Action m_onDisplayResultEnds;

    public void DislayResult(Action onDisplayResultEnds)
    {
        m_onDisplayResultEnds = onDisplayResultEnds;
        m_endResultInput.action.performed += OnEndResultInputPerformed;
    }

    private void OnEndResultInputPerformed(InputAction.CallbackContext context)
    {
        m_endResultInput.action.performed -= OnEndResultInputPerformed;
        m_onDisplayResultEnds();
    }
}
