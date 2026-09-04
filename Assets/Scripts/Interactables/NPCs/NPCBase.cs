using UnityEngine;
using EclipseGames.Player.Interaction;

public class NPCBase : MonoBehaviour, IInteractuable
{
    // Referencias a los componentes
    private NPCMovement movement;
    private NPCAnimation anim;
    private NPCDialogue dialogue;
    private NPCInteraction interaction;
    private NPCTutorialNotifier tutorialNotifier;

    [Header("Configuración (legacy)")]
    [SerializeField] private string npcName = "NPC";
    [SerializeField] private NPCDialogue.ModoDialogo modo = NPCDialogue.ModoDialogo.Interaccion;
    [SerializeField] private bool dialogoAleatorio = false;
    [TextArea]
    [SerializeField] private string[] dialogues;

    [Header("Solo Proximidad (legacy)")]
    [SerializeField] private float interactionDistance = 2f;

    [Header("Patrullaje (legacy)")]
    [SerializeField] private bool patrolEnabled = false;
    [SerializeField] private Transform patrolCenter;
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Tutorial (legacy)")]
    [SerializeField] private bool esNPCTutorial = false;
    [SerializeField] private string idNPCTutorial = "NPC1";

    private void Awake()
    {
        movement = GetComponent<NPCMovement>();
        if (movement == null)
        {
            movement = gameObject.AddComponent<NPCMovement>();
            // Copiar datos
            movement.patrolEnabled = patrolEnabled;
            movement.patrolCenter = patrolCenter;
            movement.patrolRadius = patrolRadius;
            movement.patrolSpeed = patrolSpeed;
        }

        anim = GetComponent<NPCAnimation>();
        if (anim == null)
            anim = gameObject.AddComponent<NPCAnimation>();

        dialogue = GetComponent<NPCDialogue>();
        if (dialogue == null)
        {
            dialogue = gameObject.AddComponent<NPCDialogue>();
            dialogue.npcName = npcName;
            dialogue.modo = (NPCDialogue.ModoDialogo)modo;
            dialogue.dialogoAleatorio = dialogoAleatorio;
            dialogue.dialogues = dialogues;
            dialogue.interactionDistance = interactionDistance;
        }

        interaction = GetComponent<NPCInteraction>();
        if (interaction == null)
            interaction = gameObject.AddComponent<NPCInteraction>();

        // Tutorial
        if (esNPCTutorial)
        {
            tutorialNotifier = GetComponent<NPCTutorialNotifier>();
            if (tutorialNotifier == null)
            {
                tutorialNotifier = gameObject.AddComponent<NPCTutorialNotifier>();
                tutorialNotifier.idNPCTutorial = idNPCTutorial;
            }
            // Conectar interaction si no está
            if (tutorialNotifier.interaction == null)
                tutorialNotifier.interaction = interaction;
        }
    }

    // Implementación de IInteractuable delegando en NPCInteraction
    public void Interactuar() => interaction?.Interactuar();
    public string GetPrompt() => interaction?.GetPrompt() ?? "";
    public bool PuedeInteractuar() => interaction?.PuedeInteractuar() ?? false;
    public Transform GetTransform() => transform;


    private void NotificarTutorial(string id)
    {
        TutorialGuide guia = FindFirstObjectByType<TutorialGuide>();
        if (guia != null)
            guia.NotificarNPC(id);
    }
}