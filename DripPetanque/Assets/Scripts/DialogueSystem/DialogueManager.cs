using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static Dictionary<DialogueIDWraper, EventHandler> eventHandlers = new Dictionary<DialogueIDWraper, EventHandler>();

    private void StartDialogue()
    {
    }
}
