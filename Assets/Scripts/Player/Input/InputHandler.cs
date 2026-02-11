using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    private PlayerControls playerControls;
    private Vector2 moveInput;

    // Eventos para interacción
    public System.Action OnInteract; // ← NUEVO

    public Vector2 MoveInput => moveInput;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();

        // Movimiento
        playerControls.Gameplay.Move.performed += OnMovePerformed;
        playerControls.Gameplay.Move.canceled += OnMoveCanceled;

        // Interacción - BOTÓN E o A en gamepad ← NUEVO
        playerControls.Gameplay.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Move.performed -= OnMovePerformed;
        playerControls.Gameplay.Move.canceled -= OnMoveCanceled;
        playerControls.Gameplay.Interact.performed -= OnInteractPerformed;

        playerControls.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Priorizar dirección
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            moveInput.y = 0;
        else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
            moveInput.x = 0;
        else
            moveInput.y = 0;

        playerMovement?.SetMoveDirection(moveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        playerMovement?.SetMoveDirection(Vector2.zero);
    }

    // ← NUEVO: Disparar evento de interacción
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }
}