using System;
using UnityEngine;

/// <summary>
/// A simple component to propagate the Unity trigger events
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerObject : MonoBehaviour
{
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerStayEvent;
    public event Action<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}
