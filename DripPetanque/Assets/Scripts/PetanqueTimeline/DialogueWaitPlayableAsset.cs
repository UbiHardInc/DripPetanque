using UnityEngine;
using UnityEngine.Playables;

public class DialogueWaitPlayableAsset : PlayableAsset
{
    public DialogueData DialogueData;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueWaitPlayableBehaviour>.Create(graph);

        DialogueWaitPlayableBehaviour dialogueControlBehaviour = playable.GetBehaviour();
        dialogueControlBehaviour.DialogueData = DialogueData;

        return playable;
    }
}
