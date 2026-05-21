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

    [Header("Patrullaje")]
    [SerializeField] private bool patrolEnabled = false;
    [SerializeField] private Transform patrolCenter;
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private Color colorPatrullaje = Color.green;

    private Transform player;
    private bool wasInside = false;
    private bool canInteract = true;
    private Vector2 patrolDirection;
    private Vector2 patrolCenterPos;

    private void Start()
    {
        if (patrolEnabled && patrolRadius > 0f)
        {
            patrolCenterPos = patrolCenter != null ?
                (Vector2)patrolCenter.position : (Vector2)transform.position;
            patrolDirection = Random.insideUnitCircle.normalized;
        }
    }

    private void Update()
    {
        if (patrolEnabled && patrolRadius > 0f)
            PatrolUpdate();

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

    private void PatrolUpdate()
    {
        Vector2 currentPos = transform.position;
        Vector2 nextPos = currentPos + patrolDirection * patrolSpeed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(
            currentPos,
            patrolDirection,
            patrolSpeed * Time.deltaTime,
            LayerMask.GetMask("Obstacle")
        );

        if (hit.collider != null)
        {
            patrolDirection = Vector2.Reflect(patrolDirection, hit.normal).normalized;
            return;
        }

        if (Vector2.Distance(nextPos, patrolCenterPos) > patrolRadius)
        {
            Vector2 dirAlCentro = (patrolCenterPos - currentPos).normalized;
            float angulo = Random.Range(-45f, 45f);
            patrolDirection = RotarVector(dirAlCentro, angulo);
            return;
        }

        transform.position = nextPos;
    }

    private Vector2 RotarVector(Vector2 v, float grados)
    {
        float rad = grados * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y).normalized;
    }

    public void Interactuar()
    {
        if (modo != ModoDialogo.Interaccion) return;
        if (!PuedeInteractuar() || dialogues.Length == 0) return;
        IniciarDialogoInteraccion();
        canInteract = false;
        patrolEnabled = false; 
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
        patrolEnabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (modo == ModoDialogo.Proximidad)
        {
            Gizmos.color = colorGizmo;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }

        if (patrolEnabled && patrolRadius > 0f)
        {
            Vector3 centerPos = patrolCenter != null ?
                patrolCenter.position : transform.position;
            Gizmos.color = colorPatrullaje;
            Gizmos.DrawWireSphere(centerPos, patrolRadius);
        }
    }
}