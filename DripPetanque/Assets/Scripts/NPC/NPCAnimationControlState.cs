using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimationControlState : MonoBehaviour
{
  
    private Animator m_animator;
    public StateSwitcher State;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        StateSwitch();
    }
    
    private void StateSwitch()
    {
        switch (State)
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
        }        
    }
    public enum StateSwitcher
    {
        Talking01,
        Talking02,
        Talking03,
        Talking04,
        Idle
    }
}
