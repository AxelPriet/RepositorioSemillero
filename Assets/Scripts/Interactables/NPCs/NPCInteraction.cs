using UnityEngine;
using EclipseGames.Player.Interaction;

[RequireComponent(typeof(NPCDialogue))]
public class NPCInteraction : MonoBehaviour, IInteractuable
{
    public event System.Action OnInteracted;

    private NPCDialogue dialogue;
    private bool canInteract = true;

    private void Start()
    {
        dialogue = GetComponent<NPCDialogue>();
        if (dialogue != null)
        {
            dialogue.OnDialogueEnded += () => canInteract = true;
        }
    }

    public void Interactuar()
    {
        if (!PuedeInteractuar() || dialogue == null || !dialogue.HasDialogues) return;

        canInteract = false;
        dialogue.StartFullDialogue();
        OnInteracted?.Invoke();
    }

    public string GetPrompt()
    {
        if (!PuedeInteractuar()) return "";
        return $"Hablar con {dialogue?.GetNPCName() ?? "NPC"}";
    }

    public bool PuedeInteractuar() => canInteract && dialogue != null && !dialogue.IsDialogueActive;
    public Transform GetTransform() => transform;
}