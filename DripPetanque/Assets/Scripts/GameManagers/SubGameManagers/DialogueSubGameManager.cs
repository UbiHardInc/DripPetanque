using System;
using UnityEngine;

public class DialogueSubGameManager : SubGameManager
{
    public override GameState CorrespondingState => GameState.Dialogue;

    public event Action OnDialogueEndedEvent;

    [SerializeField] private DialogueManager m_dialogueManager;

    [NonSerialized] private DialogueData m_currentDialogue;


    public override void BeginState(GameState previousState)
    {
        base.BeginState(previousState);

        m_currentDialogue = m_sharedDatas.NextDialogueToStart;
        if (m_currentDialogue == null)
        {
            Debug.LogError($"No {nameof(DialogueData)} in the shared datas : Exiting the Dialogue state");
            m_requestedGameState = previousState;
            return;
        }
        m_sharedDatas.NextDialogueToStart = null;

        m_dialogueManager.OnDialogueEnded += OnDialogueEnded;
        m_dialogueManager.StartDialogue(m_currentDialogue);
    }

    private void OnDialogueEnded()
    {
        OnDialogueEndedEvent?.Invoke();

        m_dialogueManager.OnDialogueEnded -= OnDialogueEnded;

        if (m_currentDialogue.NextStateDatas.NextGameState == GameState.None)
        {
            Debug.LogError($"The next game state of the current dialogue ({m_currentDialogue.name}) is equals to {GameState.None}\nCan't exit the current game state");
            return;
        }

        m_requestedGameState = m_currentDialogue.NextStateDatas.ApplyDatas(m_sharedDatas);
        m_currentDialogue = null;
    }
}
