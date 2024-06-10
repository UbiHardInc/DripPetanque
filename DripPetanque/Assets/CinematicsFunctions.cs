using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicsFunctions : MonoBehaviour
{
    public void StartDialogue(DialogueData dialogueData)
    {
        GameManager.Instance.ExplorationSubGameManager.StartDialogue(dialogueData);
    }
}
