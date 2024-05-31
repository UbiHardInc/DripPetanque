using UnityEngine;
using UnityEngine.Playables;

public class Interact_StartCinematic : InteractableObject
{
    [SerializeField] private string m_messageToShow;
    [SerializeField] private PlayableDirector m_cinematicToLaunch;

    public override string GetInteractionMessage()
    {
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        m_cinematicToLaunch.Play();
    }
}
