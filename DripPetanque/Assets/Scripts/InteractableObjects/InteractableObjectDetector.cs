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
        switch (m_interactableObjectsInRange.Count)
        {
            case 1:
                interactableObject = m_interactableObjectsInRange[0];
                return true;

            case > 1:
                m_interactableObjectsInRange.Sort(InteractableObjectComparison);
                interactableObject = m_interactableObjectsInRange[0];
                return true;

            case 0:
            default:
                interactableObject = null;
                return false;
        }
    }

    /// <summary>
    /// Method used to sort the <see cref="InteractableObject"/> based on the angle between the detector forward and
    /// the vector from the detector position to the interactable object position
    /// </summary>
    private int InteractableObjectComparison(InteractableObject objectA, InteractableObject objectB)
    {
        Transform detectorTransform = transform;
        Vector2 detectorPosition = detectorTransform.position.XZ();
        Vector2 detectorForward = detectorTransform.forward.XZ();

        Vector2 objAPosition = objectA.transform.position.XZ();
        Vector2 objBPosition = objectB.transform.position.XZ();

        float objAAngleish = Angleish(detectorForward, objAPosition - detectorPosition);
        float objBAngleish = Angleish(detectorForward, objBPosition - detectorPosition);

        return objBAngleish.CompareTo(objAAngleish);
    }

    private float Angleish(Vector2 from, Vector2 to)
    {
        return Vector2.Dot(from, to) / (from.sqrMagnitude * to.sqrMagnitude);
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
            _ = m_interactableObjectsInRange.RemoveAll(obj => obj == interactable);
        }
        else
        {
            Debug.LogError($"[{nameof(InteractableObjectDetector)}] All objects exiting the trigger should have a component derived from {nameof(InteractableObject)}");
        }
    }
}
