using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference m_interactionAction;
    [SerializeField] private InteractableObjectDetector m_interactableObjectDetector;
    [SerializeField] private TMP_Text m_text;

    private void Awake()
    {
        m_interactionAction.action.actionMap.Enable();
        m_interactionAction.action.performed += OnInteractionButtonPressed;
    }

    private void OnInteractionButtonPressed(InputAction.CallbackContext context)
    {
        if (m_interactableObjectDetector.GetClosestInteractableObject(out InteractableObject obj))
        {
            obj.Interact(this);
        }
    }

    private void Update()
    {
        if (m_interactableObjectDetector.GetClosestInteractableObject(out InteractableObject obj))
        {
            m_text.gameObject.SetActive(true);
            m_text.text = obj.GetInteractionMessage();
        }
        else
        {
            //m_text.gameObject.SetActive(false);
        }
    }
}
