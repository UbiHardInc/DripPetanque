using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private string _iD;
    [SerializeField] UnityEvent _eventToLaunch;

    private void OnEnable()
    {
        DialogueManager.eventHandlers[_iD] = this;
    }

    private void OnDisable()
    {
        DialogueManager.eventHandlers.Remove(_iD);
    }

    public void LaunchEvent()
    {
        _eventToLaunch?.Invoke();
    }
}
