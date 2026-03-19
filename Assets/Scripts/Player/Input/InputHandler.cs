using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    public System.Action OnMenuToggle;
    private PlayerControls playerControls;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.Gameplay.Enable();
            playerControls.Gameplay.Menu.performed += OnMenuPerformed;
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Gameplay.Disable();
            playerControls.Gameplay.Menu.performed -= OnMenuPerformed;
        }
    }
    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        OnMenuToggle?.Invoke();
    }


    public Vector2 GetMoveInput()
    {
        return playerControls?.Gameplay.Move.ReadValue<Vector2>() ?? Vector2.zero;
    }

    public PlayerControls GetControls()
    {
        return playerControls;
    }

    public bool IsRunning()
    {
        return playerControls != null && playerControls.Gameplay.Run.IsPressed();
    }
}