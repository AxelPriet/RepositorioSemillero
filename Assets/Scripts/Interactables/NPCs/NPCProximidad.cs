using UnityEngine;

public class NPCProximidad : MonoBehaviour
{
    [Header("Diálogo (ScriptableObject)")]
    [SerializeField] private NPCDialogoSO dialogo;

    [Header("Configuración de proximidad")]
    [SerializeField] private float interactionDistance = 2f;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moviendoBool = "Moviendose";  

    public System.Action OnDialogueTriggered;

    private Transform player;
    private bool wasInside = false;

    private void Start()
    {
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, false);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        if (dialogo == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool isInside = dist <= interactionDistance;

        if (isInside && !wasInside)
        {
            wasInside = true;
            string mensaje = dialogo.ObtenerLinea();
            DialogueManager.Instance?.ShowDialogue(mensaje);
            OnDialogueTriggered?.Invoke();
        }
        else if (!isInside && wasInside)
        {
            wasInside = false;
            DialogueManager.Instance?.HideDialogue();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}