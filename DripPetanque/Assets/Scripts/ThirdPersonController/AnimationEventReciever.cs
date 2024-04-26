using System;
using UnityEngine;

public class AnimationEventReciever : MonoBehaviour
{
    public event Action<AnimationEvent> OnFootstepEvent;
    public event Action<AnimationEvent> OnLandEvent;

    private void OnFootstep(AnimationEvent animationEvent)
    {
        OnFootstepEvent?.Invoke(animationEvent);
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        OnLandEvent?.Invoke(animationEvent);
    }
}
