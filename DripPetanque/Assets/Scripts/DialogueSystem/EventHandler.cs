using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    [UnityEngine.Serialization.FormerlySerializedAs("_iD")]
    [SerializeField] private string m_iD;
    [SerializeField] private UnityEvent m_beforeSentenceEvent;
    [SerializeField] private UnityEvent m_duringSentenceEvent;
    [SerializeField] private UnityEvent m_afterSentenceEvent;

    private void OnEnable()
    {
        DialogueManager.EventHandlers[m_iD] = this;
    }

    private void OnDisable()
    {
        _ = DialogueManager.EventHandlers.Remove(m_iD);
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
