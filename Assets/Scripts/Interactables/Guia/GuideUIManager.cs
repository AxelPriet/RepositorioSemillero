using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideUIManager : MonoBehaviour
{
    [Header("Personaje")]
    [SerializeField] private GameObject guideCharacter;
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(1.5f, 0f, 0f);

    [Header("Configuración")]
    [SerializeField] private string guideName = "Guideon";

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (guideCharacter != null)
            guideCharacter.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
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

        MostrarGuia();

        DialogueManager.Instance.StartDialogue(guideName, lines, () =>
        {
            onComplete?.Invoke();
        });
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
        onComplete?.Invoke();
    }
}