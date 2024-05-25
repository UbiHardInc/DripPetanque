using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(DialogueWaitPlayableAsset))]
[TrackBindingType(typeof(DialogueManager))]
public class DialogueWaitControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CinematicDialogueMixer>.Create(graph, inputCount);
    }
}
