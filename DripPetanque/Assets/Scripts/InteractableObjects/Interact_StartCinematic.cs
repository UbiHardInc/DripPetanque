using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityUtility.CustomAttributes;

public class Interact_StartCinematic : InteractableObject
{
    [SerializeField] private string m_messageToShow;
    [SerializeField] private PlayableDirector m_cinematicToLaunch;
    [SerializeField] private DialogueData m_dialogueToStart;
    [SerializeField] private List<GameObject> m_objectsToDeactivate;

    [Title("Transforms rotation")]
    [SerializeField] private Transform[] m_transformsToRotate;
    [SerializeField] private float m_rotationDuration = 1.0f;

    [NonSerialized] private Vector3 m_playerPosition;

    public override string GetInteractionMessage()
    {
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        m_cinematicToLaunch.gameObject.SetActive(true);
        GameManager.Instance.ExplorationSubGameManager.DeactivateInput();

        m_playerPosition = playerController.transform.position;

        m_cinematicToLaunch.stopped += OnCinematicStopped;
    }

    private void OnCinematicStopped(PlayableDirector director)
    {
        m_cinematicToLaunch.stopped -= OnCinematicStopped;

        m_cinematicToLaunch.gameObject.SetActive(false);

        _ = StartCoroutine(Interact_Talk.RotateTowardPlayer(m_playerPosition, m_transformsToRotate, m_rotationDuration));

        GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_dialogueToStart);
    }
}
