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
        playerMovement = FindFirstObjectByType<PlayerMovement>();

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
    // TOMAR FOTO
    // ================================

    private void OnTomarFoto(InputAction.CallbackContext context)
    {
        if (!puedeTomarFoto) return;

        Vector3 objetivoWorldPos = objetivo.position;

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

        bool dentro = rectMarco.Contains(objetivoWorldPos);

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