using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Utils;

public class InteractableObjectDetector : MonoBehaviour
{
    [SerializeField] private TriggerObject m_trigger;

    [NonSerialized] private List<InteractableObject> m_interactableObjectsInRange;

    private void Awake()
    {
        m_interactableObjectsInRange = new List<InteractableObject>();
        m_trigger.OnTriggerEnterEvent += OnObjectEnterTrigger;
        m_trigger.OnTriggerExitEvent += OnObjectExitTrigger;
    }

    public bool GetClosestInteractableObject(out InteractableObject interactableObject)
    {
        if (m_interactableObjectsInRange.Count == 0)
        {
            interactableObject = null;
            return false;
        }

        m_interactableObjectsInRange.Sort(InteractableObjectComparison);

        interactableObject = m_interactableObjectsInRange[0];
        return true;
    }

    /// <summary>
    /// Method used to sort the <see cref="InteractableObject"/> based on the angle between the detector forward and the vector from the detector position to the interatable object position
    /// </summary>
    private int InteractableObjectComparison(InteractableObject objectA, InteractableObject objectB)
    {
        Transform detectorTransform = transform;
        Vector2 detectorPosition = detectorTransform.position.XZ();
        Vector2 detectorForward = detectorTransform.forward.XZ();

        Vector2 objAPosition = objectA.transform.position.XZ();
        Vector2 objBPosition = objectB.transform.position.XZ();

        float objADot = Vector2.Dot(detectorForward, objAPosition - detectorPosition);
        float objBDot = Vector2.Dot(detectorForward, objBPosition - detectorPosition);

        return objBDot.CompareTo(objADot);
    }


    private void OnObjectEnterTrigger(Collider other)
    {
        GameObject otherGo = other.gameObject;
        if (otherGo.TryGetComponent(out InteractableObject interactable))
        {
            m_interactableObjectsInRange.Add(interactable);
        }
        else
        {
            Debug.LogError($"[{nameof(InteractableObjectDetector)}] All objects entering the trigger should have a component derived from {nameof(InteractableObject)}");
        }
    }

    private void OnObjectExitTrigger(Collider other)
    {
        GameObject otherGo = other.gameObject;
        if (otherGo.TryGetComponent(out InteractableObject interactable))
        {
            _ = m_interactableObjectsInRange.Remove(interactable);
        }
        else
        {
            Debug.LogError($"[{nameof(InteractableObjectDetector)}] All objects exiting the trigger should have a component derived from {nameof(InteractableObject)}");
        }
    }
}
