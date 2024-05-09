using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicDialogueMixer : PlayableBehaviour
{

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        DialogueManager dialogueManager = playerData as DialogueManager;

        if (!dialogueManager)
        {
            return;
        }

        int inputCount = playable.GetInputCount();

        for(int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0f)
            {
                ScriptPlayable<DialogueWaitPlayableBehaviour> inputPlayable = (ScriptPlayable<DialogueWaitPlayableBehaviour>)playable.GetInput(i);

                DialogueWaitPlayableBehaviour dialogueWaitPlayableBehaviour = inputPlayable.GetBehaviour();
                
                if (i == 0)
                {
                    dialogueManager.StartDialogue(dialogueWaitPlayableBehaviour.DialogueData);
                }
                else
                {
                    //TODO : Changer la fonction Compute Sentence pour l'appeler ici avec un int qui sera le m_sentenceIndex
                    //dialogueManager.ComputeSentences(i);
                    //dialogueManager.SentenceIndex = i;
                }

            }
        }
    }
}
