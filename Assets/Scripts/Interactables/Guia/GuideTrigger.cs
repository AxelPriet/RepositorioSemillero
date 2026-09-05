using UnityEngine;

public class GuideTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string dialogueID;

    [Header("Opciones")]
    [SerializeField] private bool triggerOnStart = false;
    [SerializeField] private bool triggerOnEnter = true;
    [SerializeField] private bool mostrarUnaVez = true;

    private bool yaTriggered = false;

    private void Start()
    {
        if (triggerOnStart)
            TriggerDialogue();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnEnter) return;
        if (!other.CompareTag("Player")) return;
        if (mostrarUnaVez && yaTriggered) return;

        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        if (string.IsNullOrEmpty(dialogueID)) return;

        yaTriggered = true;

        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.TriggerEvent(dialogueID);
        }
        else
        {
            Debug.LogWarning("GuideManager no encontrado");
        }
    }

    public void ActivarTrigger()
    {
        TriggerDialogue();
    }
}