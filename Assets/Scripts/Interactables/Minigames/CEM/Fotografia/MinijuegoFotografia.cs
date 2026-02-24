using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinijuegoFotografia : MonoBehaviour,
    IInteractuable,
    IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private MarcoArrastrable marco;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;
    [SerializeField] private float tolerancia = 100f; // Tolerancia en píxeles

    [Header("Configuración")]
    [SerializeField] private int minigameIndex = 1;

    private PlayerControls playerControls;
    private PlayerMovement playerMovement;

    private Vector3 marcoPosInicial;
    private bool enJuego = false;
    private bool puedeTomarFoto = false;

    // ================================
    // INICIALIZACIÓN
    // ================================

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Start()
    {
        if (marco != null)
        {
            marcoPosInicial = marco.transform.position;
        }
    }

    // ================================
    // INICIAR MINIJUEGO
    // ================================

    public void StartMinigame()
    {
        enJuego = true;

        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindObjectOfType<PlayerMovement>();

        playerMovement.SetCanMove(false);

        panel.SetActive(true);

        marco.transform.position = marcoPosInicial;
        marco.SetPuedeMoverse(true);

        textoInstrucciones.text = "Encuadra el objetivo y presiona ESPACIO para tomar la foto";
        puedeTomarFoto = true;

        playerControls.Gameplay.Compress.performed -= OnTomarFoto;
        playerControls.Gameplay.Compress.performed += OnTomarFoto;
    }

    // ================================
    // TOMAR FOTO - VERSIÓN CORREGIDA
    // ================================

    private void OnTomarFoto(InputAction.CallbackContext context)
    {
        if (!puedeTomarFoto) return;

        // Solución directa: usar la posición del mundo directamente
        // porque en un juego 2D orthográfico, corresponde a la pantalla
        Vector3 objetivoWorldPos = objetivo.position;

        Debug.Log($"Posición objetivo en mundo: {objetivoWorldPos}");

        // Obtener los límites del marco
        RectTransform marcoRect = marco.GetComponent<RectTransform>();
        Vector3[] esquinas = new Vector3[4];
        marcoRect.GetWorldCorners(esquinas);

        Rect rectMarco = new Rect(
            esquinas[0].x,
            esquinas[0].y,
            esquinas[2].x - esquinas[0].x,
            esquinas[2].y - esquinas[0].y
        );

        Debug.Log($"Rectángulo marco: {rectMarco}");

        // Verificar si la posición del mundo del objetivo está dentro del marco
        // OJO: Esto asume que la UI y el mundo comparten el mismo sistema de coordenadas
        bool dentro = rectMarco.Contains(objetivoWorldPos);

        Debug.Log($"¿Dentro? {dentro}");

        if (dentro)
        {
            CompleteMinigame();
        }
        else
        {
            marco.transform.position = marcoPosInicial;
            textoInstrucciones.text = "Intenta de nuevo y presiona ESPACIO";
        }
    }

    // ================================
    // COMPLETAR MINIJUEGO
    // ================================

    public void CompleteMinigame()
    {
        puedeTomarFoto = false;
        marco.SetPuedeMoverse(false);
        playerControls.Gameplay.Compress.performed -= OnTomarFoto;
        playerMovement.SetCanMove(true);
        panel.SetActive(false);
        enJuego = false;
        GameProgressManager.Instance.CompleteMinigame();
        Debug.Log("Minijuego Fotografía completado");
    }

    // ================================
    // INTERACCIÓN
    // ================================

    public void Interactuar()
    {
        if (!PuedeInteractuar()) return;
        StartMinigame();
    }

    public bool PuedeInteractuar()
    {
        return GameProgressManager.Instance.CanPlayMinigame(minigameIndex) && !enJuego;
    }

    public string GetPrompt()
    {
        if (!PuedeInteractuar())
            return "Minijuego completado";
        return "Iniciar Fotografía";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void FailMinigame() { }
}