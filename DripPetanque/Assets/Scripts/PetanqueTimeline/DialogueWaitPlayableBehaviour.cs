using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DialogueWaitPlayableBehaviour : PlayableBehaviour
{
    public DialogueData DialogueData;

    // source : https://forum.unity.com/threads/code-example-how-to-execute-logic-at-the-beginning-and-end-of-a-clip.661315/
    // POUR FAIRE UN BEHAVIOUR AU DEBUT DU CLIP ET A LA FIN

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // Execute your starting logic here, calling into a singleton for example
        Debug.Log("Clip started!");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        var duration = playable.GetDuration();
        var time = playable.GetTime();
        var count = time + info.deltaTime;

        if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately((float)time, (float)duration))
        {
            // Execute your finishing logic here:
            Debug.Log("Clip done!");
        }
        return;
    }
}
