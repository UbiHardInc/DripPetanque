using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private string _iD;
    [SerializeField] UnityEvent _eventToLaunch;

    private void OnEnable()
    {
        DialogueManager.EventHandlers[_iD] = this;
    }

    private void OnDisable()
    {
        DialogueManager.EventHandlers.Remove(_iD);
    }

    public void LaunchEvent()
    {
        _eventToLaunch?.Invoke();
    }
}
