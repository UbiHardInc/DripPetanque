using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public abstract string GetInteractionMessage();

    public abstract void Interact(PlayerController playerController);
}
