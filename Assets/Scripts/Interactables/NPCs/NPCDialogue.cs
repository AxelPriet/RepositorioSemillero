using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public enum ModoDialogo { Proximidad, Interaccion }

    [Header("Configuración")]
    [SerializeField] public string npcName = "NPC";
    [SerializeField] public ModoDialogo modo = ModoDialogo.Interaccion;
    [SerializeField] public bool dialogoAleatorio = false;
    [TextArea]
    [SerializeField] public string[] dialogues;

    [Header("Solo Proximidad")]
    [SerializeField] public float interactionDistance = 2f;

    public event System.Action OnDialogueStarted;
    public event System.Action OnDialogueEnded;

    private Transform player;
    private bool wasInside = false;
    private bool isDialogueActive = false;

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
            StartDialogue();
        }
        else if (!isInside && wasInside)
        {
            wasInside = false;
            DialogueManager.Instance?.HideDialogue();
            OnDialogueEnded?.Invoke();
        }
    }

    public void StartDialogue()
    {
        if (dialogues.Length == 0 || isDialogueActive) return;

        isDialogueActive = true;
        OnDialogueStarted?.Invoke();

        string linea = dialogoAleatorio
            ? dialogues[Random.Range(0, dialogues.Length)]
            : dialogues[0];

        DialogueManager.Instance?.ShowDialogue(linea);

        // Si es modo interacción, el diálogo se maneja desde NPCInteraction
        if (modo == ModoDialogo.Interaccion)
        {
            // Llamamos a un método para que el diálogo se cierre cuando termine
            // Se asume que DialogueManager tiene un evento de fin
            // Por ahora lo dejamos simple, pero podríamos suscribirnos.
        }
    }

    public void StartFullDialogue()
    {
        if (dialogues.Length == 0 || isDialogueActive) return;

        isDialogueActive = true;
        OnDialogueStarted?.Invoke();

        DialogueManager.Instance?.StartDialogue(npcName, dialogues, () => {
            isDialogueActive = false;
            OnDialogueEnded?.Invoke();
        });
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        DialogueManager.Instance?.HideDialogue();
        OnDialogueEnded?.Invoke();
    }

    public string GetNPCName() => npcName;
    public bool IsDialogueActive => isDialogueActive;
    public bool HasDialogues => dialogues.Length > 0;

    private void OnDrawGizmosSelected()
    {
        if (modo == ModoDialogo.Proximidad)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}