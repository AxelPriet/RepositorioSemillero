using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class MinijuegoLinterna : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Elementos del Juego")]
    [SerializeField] private RectTransform graduado;
    [SerializeField] private RectTransform linterna;
    [SerializeField] private float velocidadGraduado = 50f;

    [Header("Configuración Linterna")]
    [SerializeField] private float radioLinterna = 50f;
    [SerializeField] private float posicionYFija = 0f; 

    [Header("UI Progreso")]
    [SerializeField] private Slider sliderProgreso; 
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Configuración")]
    [SerializeField] private float tiempoRequerido = 10f;
    [SerializeField] private int minigameIndex = 4;

    // Estado del juego
    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private bool enJuego = false;
    private bool minijuegoCompletado = false;

    // Temporizadores
    private float tiempoAcumulado = 0f;

    // Posiciones límite
    private float limiteIzquierdo;
    private float limiteDerecho;
    private float limiteXLinterna; 
    private Vector2 posicionInicialGraduado;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Start()
    {
        RectTransform panelRect = graduado.parent as RectTransform;
        if (panelRect != null)
        {
            float anchoPanel = panelRect.rect.width;
            float mitadGraduado = graduado.rect.width / 2;

            limiteIzquierdo = -anchoPanel / 2 + mitadGraduado;
            limiteDerecho = anchoPanel / 2 - mitadGraduado;

            limiteXLinterna = anchoPanel / 2 - radioLinterna;
        }

        posicionInicialGraduado = new Vector2(limiteDerecho, 0);

        if (sliderProgreso != null)
        {
            sliderProgreso.minValue = 0;
            sliderProgreso.maxValue = tiempoRequerido;
            sliderProgreso.value = 0;
        }
    }

    private void Update()
    {
        if (!enJuego || minijuegoCompletado) return;

        MoverGraduado();

        ActualizarPosicionLinterna();

        VerificarDeteccion();

        if (graduado.anchoredPosition.x <= limiteIzquierdo)
        {
            ReiniciarJuego();
        }
    }

    private void MoverGraduado()
    {
        Vector2 posicion = graduado.anchoredPosition;
        posicion.x -= velocidadGraduado * Time.deltaTime;

        graduado.anchoredPosition = posicion;
    }

    private void ActualizarPosicionLinterna()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                graduado.parent as RectTransform,
                mousePos,
                null,
                out Vector2 posicionMouse
            );
            float nuevaX = Mathf.Clamp(posicionMouse.x, -limiteXLinterna, limiteXLinterna);

            linterna.anchoredPosition = new Vector2(nuevaX, posicionYFija);
        }
    }

    private void VerificarDeteccion()
    {
        float distanciaX = Mathf.Abs(linterna.anchoredPosition.x - graduado.anchoredPosition.x);
        float distanciaY = Mathf.Abs(linterna.anchoredPosition.y - graduado.anchoredPosition.y);

        bool estaDentro = (distanciaX * distanciaX + distanciaY * distanciaY) <= (radioLinterna * radioLinterna);

        if (estaDentro)
        {
            tiempoAcumulado += Time.deltaTime;

            graduado.GetComponent<Image>().color = Color.white;

            sliderProgreso.value = tiempoAcumulado;
            textoTiempo.text = $"Tiempo: {tiempoRequerido - tiempoAcumulado:F1}s";

            if (tiempoAcumulado >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoAcumulado = 0f;
            sliderProgreso.value = 0;
            textoTiempo.text = $"Tiempo: {tiempoRequerido:F0}s";

            graduado.GetComponent<Image>().color = Color.gray;
        }
    }

    private void ReiniciarJuego()
    {
        graduado.anchoredPosition = posicionInicialGraduado;
        tiempoAcumulado = 0f;
        sliderProgreso.value = 0;
        textoTiempo.text = $"Tiempo: {tiempoRequerido:F0}s";
        feedbackText.text = "Llegó al final. ¡Inténtalo de nuevo!";

        StartCoroutine(FeedbackReinicio());
    }

    private IEnumerator FeedbackReinicio()
    {
        Color colorOriginal = graduado.GetComponent<Image>().color;
        graduado.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.3f);
        graduado.GetComponent<Image>().color = colorOriginal;
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡GRADUADO!";

        float tiempo = 0;
        while (tiempo < 0.5f)
        {
            graduado.localScale = Vector3.one * (1.2f + Mathf.Sin(tiempo * 30f) * 0.1f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        CompleteMinigame();
    }

    public void StartMinigame()
    {
        enJuego = true;
        minijuegoCompletado = false;
        tiempoAcumulado = 0f;

        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.SetCanMove(false);

        panel.SetActive(true);

        graduado.anchoredPosition = posicionInicialGraduado;

        linterna.anchoredPosition = new Vector2(0, posicionYFija);

        sliderProgreso.value = 0;
        textoTiempo.text = $"Tiempo: {tiempoRequerido:F0}s";
        feedbackText.text = "Sigue al graduado con la linterna";
        textoInstrucciones.text = "Mueve el mouse horizontalmente";

        ConfigurarLinterna();
    }

    private void ConfigurarLinterna()
    {
        Image imagenLinterna = linterna.GetComponent<Image>();
        if (imagenLinterna != null)
        {
            imagenLinterna.color = new Color(1, 1, 1, 0.2f); 
        }

        linterna.sizeDelta = new Vector2(radioLinterna * 2, radioLinterna * 2);
    }

    public void CompleteMinigame()
    {
        enJuego = false;
        playerMovement.SetCanMove(true);
        panel.SetActive(false);
        GameProgressManager.Instance.CompleteMinigame();
        Debug.Log("Minijuego Linterna completado");
    }

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
        return "Seguir al graduado";
    }

    public Transform GetTransform() => transform;
    public void FailMinigame() { }
}