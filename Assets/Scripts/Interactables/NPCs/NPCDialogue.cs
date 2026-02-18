using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string[] dialogues;

    [SerializeField] private float interactionDistance = 2f;

    private Transform player;
    private bool playerInside;
    private bool dialogueTriggered;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionDistance)
        {
            if (!playerInside)
            {
                playerInside = true;
                TriggerDialogue();
            }
        }
        else
        {
            if (playerInside)
            {
                playerInside = false;
                DialogueManager.Instance.FinishAndHide();
            }
        }
    }

    void TriggerDialogue()
    {
        if (dialogues.Length == 0) return;

        int randomIndex = Random.Range(0, dialogues.Length);
        DialogueManager.Instance.ShowDialogue(dialogues[randomIndex]);
    }
}

