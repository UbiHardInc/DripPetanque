using System.Numerics;

public class ExplorationSubGameManager : SubGameManager
{
    public override GameState CorrespondingState => GameState.Exploration;

    public void StartDialogue(DialogueData dialogueData, int fromSentence = -1, int toSentence = -1)
    {
        m_sharedDatas.NextDialogueToStart = dialogueData;

        if(fromSentence != -1 && toSentence != -1)
        {
            m_sharedDatas.CustomDialogueBoundariesToDisplay = new Vector2(fromSentence, toSentence);
        }

        m_requestedGameState = GameState.Dialogue;
    }
}
