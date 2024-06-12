using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimationControlState : MonoBehaviour
{
    public enum StateSwitcher
    {
        Talking01,
        Talking02,
        Talking03,
        Talking04,
        Idle
    }

    [SerializeField] private StateSwitcher m_state;

    private Animator m_animator;

    [SerializeField] private bool m_resetOnDialogueEnd = true;

    // Start is called before the first frame update
    private void Start()
    {
        m_animator = GetComponent<Animator>();
        StateSwitch(m_state);

        if (m_resetOnDialogueEnd)
        {
            GameManager.Instance.DialogueSubGameManager.OnDialogueEndedEvent += OnDialogueEnded;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.ApplicationIsQuitting)
        {
            return;
        }

        GameManager.Instance.DialogueSubGameManager.OnDialogueEndedEvent -= OnDialogueEnded;
    }

    private void OnDialogueEnded()
    {
        StateSwitch(m_state);
    }

    public void StateSwitch(StateSwitcher wantedState)
    {
        switch (wantedState)
        {
            case StateSwitcher.Talking01:
                m_animator.SetBool("IsTalking02", false);
                m_animator.SetBool("IsTalking03", false);
                m_animator.SetBool("IsTalking04", false);
                m_animator.SetBool("IsTalking01", true);
                break;
            case StateSwitcher.Talking02:
                m_animator.SetBool("IsTalking01", false);
                m_animator.SetBool("IsTalking03", false);
                m_animator.SetBool("IsTalking04", false);
                m_animator.SetBool("IsTalking02", true);
                break;
            case StateSwitcher.Talking03:
                m_animator.SetBool("IsTalking01", false);
                m_animator.SetBool("IsTalking02", false);
                m_animator.SetBool("IsTalking04", false);
                m_animator.SetBool("IsTalking03", true);
                break;
            case StateSwitcher.Talking04:
                m_animator.SetBool("IsTalking01", false);
                m_animator.SetBool("IsTalking02", false);
                m_animator.SetBool("IsTalking03", false);
                m_animator.SetBool("IsTalking04", true);
                break;
            case StateSwitcher.Idle:
                m_animator.SetBool("IsTalking01", false);
                m_animator.SetBool("IsTalking02", false);
                m_animator.SetBool("IsTalking03", false);
                m_animator.SetBool("IsTalking04", false);
                m_animator.SetBool("IsIdle", true);
                break;
            default:
                break;
        }
    }
}
