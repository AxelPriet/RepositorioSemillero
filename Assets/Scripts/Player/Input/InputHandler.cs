using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (playerControls != null)
        {
            playerControls.Gameplay.Enable();
            playerControls.Gameplay.Menu.performed += OnMenuPerformed;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (playerControls != null)
        {
            playerControls.Gameplay.Menu.performed -= OnMenuPerformed;
            // NO deshabilitamos el Action Map aquí.
            // Así aunque el objeto se desactive momentáneamente, el input sigue vivo.
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Asegurar que el Action Map esté habilitado en cada escena nueva
        if (playerControls != null && !playerControls.Gameplay.enabled)
        {
            playerControls.Gameplay.Enable();
        }
    }

    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        OnMenuToggle?.Invoke();
    }

    public Vector2 GetMoveInput()
    {
        if (playerControls == null) return Vector2.zero;

        // Re-habilitar si por lo que sea se apagó
        if (!playerControls.Gameplay.enabled)
            playerControls.Gameplay.Enable();

        return playerControls.Gameplay.Move.ReadValue<Vector2>();
    }

    public PlayerControls GetControls() => playerControls;

    public bool IsRunning()
    {
        if (playerControls == null) return false;

        if (!playerControls.Gameplay.enabled)
            playerControls.Gameplay.Enable();

        return playerControls.Gameplay.Run.IsPressed();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}