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
        BuscarPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            BuscarPlayer(); 
            if (player == null) return; 
        }

        float distance = Vector2.Distance(transform.position, player.position);
        bool isInside = distance <= interactionDistance;

        if (isInside && !wasInside)
        {
            wasInside = true;
            ShowRandomDialogue();
        }
        else if (!isInside && wasInside)
        {
            wasInside = false;
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.HideDialogue();
        }
    }

    void BuscarPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void ShowRandomDialogue()
    {
        if (dialogues.Length == 0) return;

        int randomIndex = Random.Range(0, dialogues.Length);
        currentDialogue = dialogues[randomIndex];

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue(currentDialogue);
    }
}