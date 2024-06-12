using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicsFunctions : MonoBehaviour
{
    private DialogueData m_currentDialoguePlayed;

    public void StartDialogue(DialogueData dialogueData)
    {
        m_currentDialoguePlayed = dialogueData;
        GameManager.Instance.ExplorationSubGameManager.StartDialogue(dialogueData);
    }

    public void DeactivateInput()
    {
        GameManager.Instance.ExplorationSubGameManager.DeactivateInput();
    }

    public void ActiveInfoPanel(bool value)
    {
        CanvasManager.Instance.InfoPanel.gameObject.SetActive(value);
    }
}
