using UnityEngine;
using UnityEngine.InputSystem; // ← AÑADE ESTO

public class MinijuegoSelector : MonoBehaviour
{
    [SerializeField] private MinijuegoFotografia foto;
    [SerializeField] private MinijuegoRadio radio;
    [SerializeField] private MinijuegoCroma croma;
    [SerializeField] private MinijuegoLinterna linterna;
    [SerializeField] private MinijuegoBaloncesto basket;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = InputHandler.Instance.GetControls();
    }

    private void Update()
    {
        // Usar el Input System en lugar de Input.GetKeyDown
        if (Keyboard.current.digit1Key.wasPressedThisFrame && foto != null)
            foto.Interactuar();

        if (Keyboard.current.digit2Key.wasPressedThisFrame && radio != null)
            radio.Interactuar();

        if (Keyboard.current.digit3Key.wasPressedThisFrame && croma != null)
            croma.Interactuar();

        if (Keyboard.current.digit4Key.wasPressedThisFrame && linterna != null)
            linterna.Interactuar();

        if (Keyboard.current.digit5Key.wasPressedThisFrame && basket != null)
            basket.Interactuar();

        // Reiniciar progreso con tecla 0
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Progreso reiniciado");
        }
    }
}