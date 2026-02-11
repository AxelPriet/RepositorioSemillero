using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteraction player);
    string GetInteractionPrompt();
    bool CanInteract();
    Transform GetTransform(); 
}