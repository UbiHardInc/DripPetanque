using UnityEngine.InputSystem;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif
public class HoldAndReleaseInteraction : IInputInteraction
{
    private enum HoldAndReleaseState
    {
        Released,
        Held,
    }

    private HoldAndReleaseState m_currentState = HoldAndReleaseState.Released;


    static HoldAndReleaseInteraction()
    {
        InputSystem.RegisterInteraction<HoldAndReleaseInteraction>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Will execute the static constructor as a side effect.
    }

    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }

        float inputValue = context.ReadValue<float>();

        switch (m_currentState)
        {
            case HoldAndReleaseState.Released:
                if (inputValue > 0.9f)
                {
                    context.Performed();
                    m_currentState = HoldAndReleaseState.Held;
                }
                break;
            case HoldAndReleaseState.Held:
                if (inputValue < 0.1f)
                {
                    context.Performed();
                    m_currentState = HoldAndReleaseState.Released;
                }
                break;
            default:
                break;
        }
    }

    public void Reset()
    {
        m_currentState = HoldAndReleaseState.Released;
    }
}