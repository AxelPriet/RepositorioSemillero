using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("UI Progreso")]
    [SerializeField] private Image barraProgreso;
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
    private Rect areaJuego;

    private float limiteIzquierdo;
    private float limiteDerecho;
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
        }

        posicionInicialGraduado = new Vector2(limiteDerecho, 0);
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            graduado.parent as RectTransform,
            Input.mousePosition,
            null,
            out Vector2 posicionMouse
        );

        linterna.anchoredPosition = posicionMouse;
    }

    private void VerificarDeteccion()
    {
        float distancia = Vector2.Distance(linterna.anchoredPosition, graduado.anchoredPosition);

        bool estaDentro = distancia <= radioLinterna;

        if (estaDentro)
        {
            tiempoAcumulado += Time.deltaTime;

            graduado.GetComponent<Image>().color = Color.white;

            float progreso = tiempoAcumulado / tiempoRequerido;
            barraProgreso.fillAmount = progreso;
            textoTiempo.text = $"Tiempo: {tiempoRequerido - tiempoAcumulado:F1}s";

            if (tiempoAcumulado >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoAcumulado = 0f;
            barraProgreso.fillAmount = 0;
            textoTiempo.text = $"Tiempo: {tiempoRequerido:F0}s";

            graduado.GetComponent<Image>().color = Color.gray;
        }
    }

    private void ReiniciarJuego()
    {
        graduado.anchoredPosition = posicionInicialGraduado;
        tiempoAcumulado = 0f;
        barraProgreso.fillAmount = 0;
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

        linterna.anchoredPosition = Vector2.zero;

        barraProgreso.fillAmount = 0;
        textoTiempo.text = $"Tiempo: {tiempoRequerido:F0}s";
        feedbackText.text = "Sigue al graduado con la linterna";
        textoInstrucciones.text = "Mantén el mouse sobre él";

        ConfigurarLinterna();
    }

    private void ConfigurarLinterna()
    {
        Image imagenLinterna = linterna.GetComponent<Image>();
        if (imagenLinterna != null)
        {
            imagenLinterna.color = new Color(1, 1, 1, 0.2f); 
        }

        // Ajustar tamaño al radio
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

    // Interfaz IInteractuable
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