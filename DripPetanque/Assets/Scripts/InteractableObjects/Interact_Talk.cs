using System;
using System.Collections;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;

public class Interact_Talk : InteractableObject
{
    [SerializeField] private string m_messageToShow;
    [SerializeField] private DialogueData m_dialogueToLaunch;

    [Title("Transforms rotation")]
    [SerializeField] private Transform[] m_transformsToRotate;
    [SerializeField] private float m_rotationDuration = 1.0f;

    [Title("Animation")]
    [SerializeField] private bool m_changeState;
    [SerializeField] private NPCAnimationControlState m_controlState;
    [SerializeField] private NPCAnimationControlState.StateSwitcher m_talingState;

    public override string GetInteractionMessage()
    {
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_dialogueToLaunch);

        if (m_changeState)
        {
            m_controlState.StateSwitch(m_talingState);
        }

        if (m_transformsToRotate.Length > 0)
        {
            _ = StartCoroutine(RotateTowardPlayer(playerController.transform.position, m_transformsToRotate, m_rotationDuration));
        }
    }

    public static IEnumerator RotateTowardPlayer(Vector3 playerPosition, Transform[] transformsToRotate, float duration)
    {
        int transformsCount = transformsToRotate.Length;
        (float startRotation, float targetRotation)[] transformsRotations = new (float, float)[transformsCount];
        
        for (int i = 0; i < transformsCount; i++)
        {
            Transform transform = transformsToRotate[i];
            Vector3 transformToTarget = Vector3.ProjectOnPlane(playerPosition - transform.position, transform.up);
            transformsRotations[i] = (transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y + Vector3.SignedAngle(transform.forward, transformToTarget, transform.up));
        }

        Timer rotationTimer = new Timer(duration, false);
        rotationTimer.Start();

        while(!rotationTimer.Update(Time.deltaTime))
        {
            for (int i = 0; i < transformsCount; i++)
            {
                (float startRotation, float targetRotation) = transformsRotations[i];
                float lerpFactor = MathUtils.Smoothstep(rotationTimer.Progress);

                transformsToRotate[i].rotation = Quaternion.Euler(0.0f, Mathf.Lerp(startRotation, targetRotation, lerpFactor), 0.0f);
            }

            yield return null;
        }
    }
}
