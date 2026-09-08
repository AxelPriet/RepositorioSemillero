using UnityEngine;
using EclipseGames.Player.Interaction;

public class NPCInteraccion : MonoBehaviour, IInteractuable
{
    [Header("Diálogo (ScriptableObject)")]
    [SerializeField] private NPCDialogoSO dialogo;

    [Header("Movimiento")]
    [SerializeField] private NPCWaypointMovement movement;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moviendoBool = "Moviendose";   

    private bool canInteract = true;
    private bool isDialogueActive = false;

    public System.Action OnInteracted;

    private void Start()
    {
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, false);
        }
    }

    public void Interactuar()
    {
        if (!PuedeInteractuar() || dialogo == null || dialogo.Lineas.Length == 0) return;

        canInteract = false;
        isDialogueActive = true;

        if (movement != null)
            movement.StopMovement();

        if (animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, false);
        }

        DialogueManager.Instance?.StartDialogue(
            dialogo.NPCName,
            dialogo.Lineas,
            OnDialogueEnd
        );

        OnInteracted?.Invoke();
    }

    private void OnDialogueEnd()
    {
        canInteract = true;
        isDialogueActive = false;

        if (movement != null)
            movement.ResumeMovement();

        if (movement != null && movement.IsMoving && animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, true);
        }
    }

    public string GetPrompt()
    {
        //No mostrar prompt si las interacciones están bloqueadas
        if (InteractionBlocker.Instance != null && InteractionBlocker.Instance.IsBlocked)
            return "";

        return $"Hablar con {dialogo?.NPCName ?? "NPC"}";
    }
    public bool PuedeInteractuar()
    {
        //No se puede interactuar si está bloqueado
        if (InteractionBlocker.Instance != null && InteractionBlocker.Instance.IsBlocked)
            return false;

        return canInteract && !isDialogueActive && dialogo != null;
    }
    public Transform GetTransform() => transform;
}