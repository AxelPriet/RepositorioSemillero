using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinijuegoRCP : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("UI")]
    public GameObject panel;
    public RectTransform indicador;
    public RectTransform zonaVerde;
    [SerializeField] private TMPro.TextMeshProUGUI textoPuntos;
    [SerializeField] private TMPro.TextMeshProUGUI textoErrores;

    [Header("Configuración")]
    public float velocidad = 200f;
    public int puntosNecesarios = 5;
    [SerializeField] private float tiempoPausa = 0.3f;

    private float direccion = 1f;
    private float minY;
    private float maxY;
    private int puntos = 0;
    private int errores = 0;
    private bool enPausa = false;

    private InputHandler inputHandler;
    private PlayerControls playerControls;
    private PlayerMovement playerMovement;

    [SerializeField] private int minigameIndex = 0;

    void Start()
    {
        float alturaBarra = ((RectTransform)indicador.parent).rect.height;
        minY = -alturaBarra / 2 + (indicador.rect.height / 2);
        maxY = alturaBarra / 2 - (indicador.rect.height / 2);
        panel.SetActive(false);
    }

    private void Awake()
    {
        inputHandler = InputHandler.Instance;
    }

    void Update()
    {
        if (panel.activeSelf)
            MoverIndicador();
    }

    private void ActualizarUI()
    {
        textoPuntos.text = "Aciertos: " + puntos;
        textoErrores.text = "Errores: " + errores;
    }

    public void Interactuar()
    {
        if (!PuedeInteractuar()) return;
        StartMinigame();
    }

    public string GetPrompt()
    {
        if (!PuedeInteractuar())
            return "Minijuego completado";
        return "Iniciar RCP";
    }

    public bool PuedeInteractuar()
    {
        return GameProgressManager.Instance.CanPlayMinigame(minigameIndex) && panel.activeSelf == false;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void StartMinigame()
    {
        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        panel.SetActive(true);

        puntos = 0;
        errores = 0;
        ActualizarUI();

        playerControls.Gameplay.Compress.performed += OnCompress;
    }

    public void CompleteMinigame()
    {
        playerControls.Gameplay.Compress.performed -= OnCompress;

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        panel.SetActive(false);
        GameProgressManager.Instance.CompleteMinigame();
        Debug.Log("Minijuego completado, puedes pasar al siguiente");
    }

    public void FailMinigame()
    {
        playerControls.Gameplay.Compress.performed -= OnCompress;

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        panel.SetActive(false);
        Debug.Log("Minijuego fallido, debes intentarlo de nuevo para avanzar");
    }

    private void OnCompress(InputAction.CallbackContext context)
    {
        if (panel.activeSelf && !enPausa)
            VerificarZona();
    }

    void VerificarZona()
    {
        float indicadorY = indicador.anchoredPosition.y;
        float zonaMin = zonaVerde.anchoredPosition.y - (zonaVerde.rect.height / 2);
        float zonaMax = zonaVerde.anchoredPosition.y + (zonaVerde.rect.height / 2);

        if (indicadorY >= zonaMin && indicadorY <= zonaMax)
        {
            puntos++;
            Debug.Log("¡Buena compresión! Puntos: " + puntos);

            StartCoroutine(PausaIndicador());
            ActualizarUI();

            if (puntos >= puntosNecesarios)
            {
                CompleteMinigame();
            }
        }
        else
        {
            errores++;
            Debug.Log("Compresión incorrecta");
            ActualizarUI();

            if (errores >= 3)
            {
                FailMinigame();
            }
        }
    }

    void MoverIndicador()
    {
        if (enPausa) return;

        Vector2 pos = indicador.anchoredPosition;
        pos.y += direccion * velocidad * Time.deltaTime;

        if (pos.y >= maxY)
        {
            pos.y = maxY;
            direccion = -1;
        }
        else if (pos.y <= minY)
        {
            pos.y = minY;
            direccion = 1;
        }

        indicador.anchoredPosition = pos;
    }

    private IEnumerator PausaIndicador()
    {
        enPausa = true;
        yield return new WaitForSeconds(tiempoPausa);
        enPausa = false;
    }
}