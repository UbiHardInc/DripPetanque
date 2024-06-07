using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple component to propagate the Unity trigger events
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerObject : MonoBehaviour
{
    [SerializeField] private UnityEvent m_onTriggerEnter;
    [SerializeField] private UnityEvent m_onTriggerStay;
    [SerializeField] private UnityEvent m_onTriggerExit;
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerStayEvent;
    public event Action<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
        m_onTriggerEnter?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
        m_onTriggerStay?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
        m_onTriggerExit?.Invoke();
    }
}
