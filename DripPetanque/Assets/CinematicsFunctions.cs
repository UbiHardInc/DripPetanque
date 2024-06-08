using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicsFunctions : MonoBehaviour
{
    private DialogueManager m_dialogueManager;

    private void Start()
    {
        m_dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        m_dialogueManager.StartDialogue(dialogueData);
    }
}
