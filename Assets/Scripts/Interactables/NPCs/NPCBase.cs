using UnityEngine;
using EclipseGames.Player.Interaction;

public class NPCBase : MonoBehaviour, IInteractuable
{
    public enum ModoDialogo { Proximidad, Interaccion }

    [Header("Configuración")]
    [SerializeField] private string npcName = "NPC";
    [SerializeField] private ModoDialogo modo = ModoDialogo.Interaccion;
    [SerializeField] private bool dialogoAleatorio = false;

    [TextArea]
    [SerializeField] private string[] dialogues;

    [Header("Solo Proximidad")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Color colorGizmo = Color.yellow;

    private Transform player;
    private bool wasInside = false;
    private bool canInteract = true;

    private void Start()
    {
    }

    private void Update()
    {
        if (modo != ModoDialogo.Proximidad) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        bool isInside = dist <= interactionDistance;

        if (isInside && !wasInside)
        {
            wasInside = true;
            IniciarDialogoProximidad();
        }
        else if (!isInside && wasInside)
        {
            wasInside = false;
            DialogueManager.Instance?.HideDialogue();
        }
    }

    public void Interactuar()
    {
        if (modo != ModoDialogo.Interaccion) return;
        if (!PuedeInteractuar() || dialogues.Length == 0) return;
        IniciarDialogoInteraccion(); 
        canInteract = false;
    }

    public string GetPrompt() => $"Hablar con {npcName}";
    public bool PuedeInteractuar() => canInteract && modo == ModoDialogo.Interaccion;
    public Transform GetTransform() => transform;

    private void IniciarDialogoProximidad()
    {
        if (dialogues.Length == 0) return;

        string linea = dialogoAleatorio
            ? dialogues[Random.Range(0, dialogues.Length)]
            : dialogues[0];

        DialogueManager.Instance?.ShowDialogue(linea);
    }

    private void IniciarDialogoInteraccion()
    {
        if (dialogues.Length == 0) return;

        DialogueManager.Instance?.StartDialogue(npcName, dialogues, OnDialogueEnd);
    }

    private void OnDialogueEnd()
    {
        canInteract = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (modo != ModoDialogo.Proximidad) return;
        Gizmos.color = colorGizmo;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}