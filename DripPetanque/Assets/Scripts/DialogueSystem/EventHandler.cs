using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private string _iD;
    [SerializeField] UnityEvent m_beforeSentenceEvent;
    [SerializeField] UnityEvent m_duringSentenceEvent;
    [SerializeField] UnityEvent m_afterSentenceEvent;

    private void OnEnable()
    {
        DialogueManager.EventHandlers[_iD] = this;
    }

    private void OnDisable()
    {
        DialogueManager.EventHandlers.Remove(_iD);
    }

    public void LaunchBeforeSentenceEvent()
    {
        m_beforeSentenceEvent?.Invoke();
    }

    public void LaunchDuringSentenceEvent()
    {
        m_duringSentenceEvent?.Invoke();
    }

    public void LaunchAfterSentenceEvent()
    {
        m_afterSentenceEvent?.Invoke();
    }
}
