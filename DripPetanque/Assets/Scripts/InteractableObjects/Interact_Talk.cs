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

    public override string GetInteractionMessage()
    {
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_dialogueToLaunch);

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
            Vector3 transformToTarget = playerPosition - transform.position;
            transformsRotations[i] = (transform.rotation.eulerAngles.y, (Quaternion.FromToRotation(transform.forward, transformToTarget).eulerAngles.y - 360) % 360);
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
