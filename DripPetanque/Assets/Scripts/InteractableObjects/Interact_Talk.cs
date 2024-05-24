using UnityEngine;

public class Interact_Talk : InteractableObject
{
    [SerializeField] private string m_messageToShow;
    [SerializeField] private DialogueData m_dialogueToLaunch;

    public override string GetInteractionMessage()
    {
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_dialogueToLaunch);
    }
}
