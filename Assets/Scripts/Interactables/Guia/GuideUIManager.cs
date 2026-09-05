using UnityEngine;

public class GuideUIManager : MonoBehaviour
{
    [Header("Personaje")]
    [SerializeField] private GameObject guideCharacter;
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(1.5f, 0f, 0f);

    [Header("Configuración")]
    [SerializeField] private string guideName = "A.A.V.";

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (guideCharacter != null)
            guideCharacter.SetActive(false);
    }

    public void MostrarGuia()
    {
        if (guideCharacter == null) return;

        if (player != null)
            guideCharacter.transform.position = player.position + offsetFromPlayer;

        guideCharacter.SetActive(true);
    }

    public void OcultarGuia()
    {
        if (guideCharacter != null)
            guideCharacter.SetActive(false);
    }

    public void MostrarDialogo(string[] lines, System.Action onComplete = null)
    {
        if (DialogueManager.Instance == null)
        {
            onComplete?.Invoke();
            return;
        }

        DialogueManager.Instance.StartDialogue(guideName, lines, onComplete);
    }

    public void MostrarDialogoConID(string dialogueID, GuideDialogueSO[] dialogues, System.Action onComplete = null)
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue != null && dialogue.DialogueID == dialogueID)
            {
                MostrarDialogo(dialogue.Lines, onComplete);
                return;
            }
        }

        Debug.LogWarning($"No se encontró diálogo con ID: {dialogueID}");
        onComplete?.Invoke();
    }
}