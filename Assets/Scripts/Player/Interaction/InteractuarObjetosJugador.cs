using EclipseGames.Player.Interaction;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractuarObjetosJugador : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [SerializeField] private Transform controladorInteractuar;
    [SerializeField] private Vector2 dimensionesCaja = new Vector2(1.5f, 1.5f);
    [SerializeField] private LayerMask capasInteractuables;

    [Header("UI Interaction")]
    [SerializeField] private GameObject prefabPrompt; // Sprite "E"

    // Diccionario para prompts activos
    private Dictionary<IInteractuable, GameObject> promptsActivos = new Dictionary<IInteractuable, GameObject>();

    // Referencias
    private PlayerControls playerControls;
    private IInteractuable interactuableActual;

    private void Awake()
    {
        playerControls = new PlayerControls();

        if (controladorInteractuar == null)
            controladorInteractuar = transform;
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
        playerControls.Gameplay.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Interact.performed -= OnInteractPerformed;
        playerControls.Gameplay.Disable();

        LimpiarTodosPrompts();
    }

    private void Update()
    {
        DetectarInteractuables();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Interactuar();
    }

    private void Interactuar()
    {
        Collider2D[] objetosTocados = Physics2D.OverlapBoxAll(
            controladorInteractuar.position,
            dimensionesCaja,
            0f,
            capasInteractuables
        );

        foreach (Collider2D objeto in objetosTocados)
        {
            if (objeto.TryGetComponent(out IInteractuable interactuable))
            {
                if (interactuable.PuedeInteractuar())
                {
                    Debug.Log($"🎯 Interactuando con: {objeto.name}");
                    interactuable.Interactuar();
                    break;
                }
            }
        }
    }

    private void DetectarInteractuables()
    {
        Collider2D[] objetosTocados = Physics2D.OverlapBoxAll(
            controladorInteractuar.position,
            dimensionesCaja,
            0f,
            capasInteractuables
        );

        HashSet<IInteractuable> interactuablesFrame = new HashSet<IInteractuable>();

        // Detectar nuevos interactuables
        foreach (Collider2D objeto in objetosTocados)
        {
            if (objeto.TryGetComponent(out IInteractuable interactuable) && interactuable.PuedeInteractuar())
            {
                interactuablesFrame.Add(interactuable);

                if (!promptsActivos.ContainsKey(interactuable))
                {
                    CrearPrompt(interactuable);
                }
            }
        }

        // Remover prompts que ya no están en la caja
        List<IInteractuable> aRemover = new List<IInteractuable>();
        foreach (var kvp in promptsActivos)
        {
            if (!interactuablesFrame.Contains(kvp.Key))
            {
                aRemover.Add(kvp.Key);
            }
        }

        foreach (var interactuable in aRemover)
        {
            DestruirPrompt(interactuable);
        }
    }

    private void CrearPrompt(IInteractuable interactuable)
    {
        if (prefabPrompt == null) return;

        GameObject prompt = Instantiate(prefabPrompt);
        prompt.name = $"Prompt_{interactuable.GetTransform().name}";

        var componentePrompt = prompt.GetComponent<InteractionPrompt>();
        if (componentePrompt != null)
        {
            componentePrompt.SetTarget(interactuable.GetTransform());
            componentePrompt.SetPlayer(transform);
        }

        promptsActivos[interactuable] = prompt;
    }

    private void DestruirPrompt(IInteractuable interactuable)
    {
        if (promptsActivos.TryGetValue(interactuable, out GameObject prompt))
        {
            Destroy(prompt);
            promptsActivos.Remove(interactuable);
        }
    }

    private void LimpiarTodosPrompts()
    {
        foreach (var prompt in promptsActivos.Values)
        {
            if (prompt != null)
                Destroy(prompt);
        }
        promptsActivos.Clear();
    }

    private void OnDrawGizmos()
    {
        if (controladorInteractuar != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(controladorInteractuar.position, dimensionesCaja);
        }
    }
}