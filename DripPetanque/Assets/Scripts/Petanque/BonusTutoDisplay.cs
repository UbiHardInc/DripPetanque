using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtility.SerializedDictionary;

public class BonusTutoDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference m_endBonusTutoInput;

    [NonSerialized] private Action m_bonusTutoEndCallback;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void DislayBonusTuto(Action bonusTutoEndCallback)
    {
        gameObject.SetActive(true);
        m_bonusTutoEndCallback = bonusTutoEndCallback;
        m_endBonusTutoInput.action.performed += OnEndBonusTutoInputPerformed;
    }

    private void OnEndBonusTutoInputPerformed(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        m_endBonusTutoInput.action.performed -= OnEndBonusTutoInputPerformed;
        m_bonusTutoEndCallback();
    }

}
