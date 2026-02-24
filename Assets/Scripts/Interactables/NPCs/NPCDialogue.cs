using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string[] dialogues;

    [SerializeField] private float interactionDistance = 2f;

    private Transform player;
    private bool wasInside = false;
    private string currentDialogue = "";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        bool isInside = distance <= interactionDistance;

        if (isInside && !wasInside)
        {
            // Entró al rango - mostrar diálogo
            wasInside = true;
            ShowRandomDialogue();
        }
        else if (!isInside && wasInside)
        {
            // Salió del rango - ocultar inmediatamente
            wasInside = false;
            DialogueManager.Instance.HideDialogue();
        }
    }

    void ShowRandomDialogue()
    {
        if (dialogues.Length == 0) return;

        int randomIndex = Random.Range(0, dialogues.Length);
        currentDialogue = dialogues[randomIndex];
        DialogueManager.Instance.ShowDialogue(currentDialogue);
    }
}