public class ExplorationSubGameManager : SubGameManager
{
    public override GameState CorrespondingState => GameState.Exploration;

    public void StartDialogue(DialogueData dialogueData)
    {
        m_sharedDatas.NextDialogueToStart = dialogueData;

        m_requestedGameState = GameState.Dialogue;
    }
}
