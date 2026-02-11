using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform interactionPoint; // Punto desde donde interactúa

    [Header("Estado")]
    [SerializeField] private IInteractable currentInteractable;

    // Referencias
    private InputHandler inputHandler;
    private PlayerMovement playerMovement;

    // Eventos para otros sistemas
    public System.Action<IInteractable> OnInteractableEnter;
    public System.Action<IInteractable> OnInteractableExit;
    public System.Action OnInteractionPerformed;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerMovement = GetComponent<PlayerMovement>();

        if (interactionPoint == null)
            interactionPoint = transform;
    }

    private void OnEnable()
    {
        // Suscribirse al input de interacción
        if (inputHandler != null)
            inputHandler.OnInteract += PerformInteraction;
    }

    private void OnDisable()
    {
        if (inputHandler != null)
            inputHandler.OnInteract -= PerformInteraction;
    }

    private void Update()
    {
        DetectInteractables();
    }

    private void DetectInteractables()
    {
        // Detectar todos los interactuables en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            interactionPoint.position,
            interactionRange,
            interactableLayer
        );

        IInteractable nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null && interactable.CanInteract())
            {
                float distance = Vector2.Distance(
                    interactionPoint.position,
                    hit.transform.position
                );

                // Encontrar el más cercano
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }

        // Si cambió el interactuable actual
        if (nearestInteractable != currentInteractable)
        {
            if (currentInteractable != null)
                OnInteractableExit?.Invoke(currentInteractable);

            currentInteractable = nearestInteractable;

            if (currentInteractable != null)
                OnInteractableEnter?.Invoke(currentInteractable);
        }
    }

    public void PerformInteraction()
    {
        if (currentInteractable != null && currentInteractable.CanInteract())
        {
            currentInteractable.Interact(this);
            OnInteractionPerformed?.Invoke();
        }
    }

    // Obtener el interactuable actual para UI
    public IInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }

    // Visualización del rango en editor
    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRange);
        }
    }
}
