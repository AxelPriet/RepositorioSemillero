using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Configuración del NPC")]
    [SerializeField] private string npcName = "Aldeano";
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private Sprite portraitSprite;

    [Header("Estado")]
    [SerializeField] private bool canInteract = true;

    // Implementación de IInteractable
    public void Interact(PlayerInteraction player)
    {
        if (!canInteract) return;

        // Aquí llamarías a tu sistema de diálogo
        Debug.Log($"Hablando con {npcName}");

        // Ejemplo simple:
        foreach (string line in dialogueLines)
        {
            Debug.Log($"{npcName}: {line}");
        }

        // Evento para sistema de diálogo
        //DialogueEvents.OnDialogueStart?.Invoke(this);
    }

    public string GetInteractionPrompt()
    {
        return $"Hablar con {npcName} (E)";
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // Métodos públicos para habilitar/deshabilitar
    public void SetInteractable(bool value)
    {
        canInteract = value;
    }
}