using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    private PlayerControls playerControls;
    private Vector2 moveInput;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
        playerControls.Gameplay.Move.performed += OnMovePerformed;
        playerControls.Gameplay.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Move.performed -= OnMovePerformed;
        playerControls.Gameplay.Move.canceled -= OnMoveCanceled;

        playerControls.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

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

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}