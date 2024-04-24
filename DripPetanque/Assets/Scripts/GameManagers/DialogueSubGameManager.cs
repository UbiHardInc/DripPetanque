using System;
using UnityEngine;

public class DialogueSubGameManager : SubGameManager
{
    public override GameState CorrespondingState => GameState.Dialogue;

    [SerializeField] private DialogueManager m_dialogueManager;

    [NonSerialized] private DialogueData m_dialogueToStart;


    public override void BeginState(GameState previousState)
    {
        base.BeginState(previousState);

        m_dialogueToStart = m_sharedDatas.NextDialogueToStart;
        if (m_dialogueToStart == null)
        {
            Debug.LogError($"No {nameof(DialogueData)} in the shared datas : Exiting the Dialogue state");
            m_requestedGameState = previousState;
            return;
        }
        m_sharedDatas.NextDialogueToStart = null;

        m_dialogueManager.OnDialogueEnded += OnDialogueEnded;
        m_dialogueManager.StartDialogue(m_dialogueToStart);
    }

    private void OnDialogueEnded()
    {
        m_dialogueManager.OnDialogueEnded -= OnDialogueEnded;

        if (m_dialogueToStart.NextGameState == GameState.None)
        {
            Debug.LogError($"The next game state of the current dialogue ({m_dialogueToStart.name}) is equals to {GameState.None}\nCan't exit the current game state");
            return;
        }

        m_requestedGameState = m_dialogueToStart.NextGameState;
        m_dialogueToStart = null;
    }
}
