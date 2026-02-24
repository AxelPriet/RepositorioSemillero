using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MinijuegoCroma : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Perillas")]
    [SerializeField] private PerillaRadio perillaRoja;
    [SerializeField] private PerillaRadio perillaVerde;
    [SerializeField] private PerillaRadio perillaAzul;

    [Header("Luces")]
    [SerializeField] private Image luzRoja;
    [SerializeField] private Image luzVerde;
    [SerializeField] private Image luzAzul;

    [Header("Visualización")]
    [SerializeField] private Image colorObjetivo;
    [SerializeField] private Image colorActual;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Configuración")]
    [SerializeField] private float tolerancia = 0.1f;
    [SerializeField] private int minigameIndex = 3;
    [SerializeField] private float tiempoRequerido = 2f; // Segundos que debe mantenerse

    // Valores objetivo (se generan aleatoriamente)
    private float valorObjetivoRojo;
    private float valorObjetivoVerde;
    private float valorObjetivoAzul;

    // Estado del juego
    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private bool enJuego = false;
    private bool minijuegoCompletado = false;

    // Cuenta atrás
    private float tiempoCorrecto = 0f;
    private bool cuentaAtrasActivada = false;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!enJuego || minijuegoCompletado) return;

        // Actualizar brillo de las luces según perillas
        ActualizarLuces();

        // Actualizar color mezclado
        ActualizarColorMezclado();

        // Verificar si está correcto
        VerificarMezclaEnTiempoReal();
    }

    private void ActualizarLuces()
    {
        // Cambiar intensidad de las luces (alpha o color multiplicado)
        Color colorRojo = Color.red * perillaRoja.ValorNormalizado;
        Color colorVerde = Color.green * perillaVerde.ValorNormalizado;
        Color colorAzul = Color.blue * perillaAzul.ValorNormalizado;

        luzRoja.color = new Color(colorRojo.r, colorRojo.g, colorRojo.b, 1f);
        luzVerde.color = new Color(colorVerde.r, colorVerde.g, colorVerde.b, 1f);
        luzAzul.color = new Color(colorAzul.r, colorAzul.g, colorAzul.b, 1f);
    }

    private void ActualizarColorMezclado()
    {
        // Mezclar los colores RGB
        float r = perillaRoja.ValorNormalizado;
        float g = perillaVerde.ValorNormalizado;
        float b = perillaAzul.ValorNormalizado;

        colorActual.color = new Color(r, g, b);
    }

    private void VerificarMezclaEnTiempoReal()
    {
        float diferenciaR = Mathf.Abs(perillaRoja.ValorNormalizado - valorObjetivoRojo);
        float diferenciaG = Mathf.Abs(perillaVerde.ValorNormalizado - valorObjetivoVerde);
        float diferenciaB = Mathf.Abs(perillaAzul.ValorNormalizado - valorObjetivoAzul);

        bool rojoCorrecto = diferenciaR < tolerancia;
        bool verdeCorrecto = diferenciaG < tolerancia;
        bool azulCorrecto = diferenciaB < tolerancia;
        bool todoCorrecto = rojoCorrecto && verdeCorrecto && azulCorrecto;

        if (todoCorrecto)
        {
            // Iniciar o continuar cuenta atrás
            tiempoCorrecto += Time.deltaTime;

            if (!cuentaAtrasActivada)
            {
                cuentaAtrasActivada = true;
            }

            // Actualizar texto con cuenta atrás
            float tiempoRestante = tiempoRequerido - tiempoCorrecto;
            feedbackText.text = $"¡Color perfecto! Mantén {tiempoRestante:F1}s";

            // Cambiar feedback visual
            colorActual.transform.localScale = Vector3.one * (1f + (tiempoCorrecto / tiempoRequerido * 0.2f));

            // Verificar si ya cumplió el tiempo
            if (tiempoCorrecto >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            // Resetear cuenta atrás si se desajusta
            if (cuentaAtrasActivada)
            {
                cuentaAtrasActivada = false;
                StartCoroutine(FeedbackError());
            }

            tiempoCorrecto = 0f;
            colorActual.transform.localScale = Vector3.one;

            // Feedback de qué color ajustar
            if (!rojoCorrecto && !verdeCorrecto && !azulCorrecto)
                feedbackText.text = "Ajusta todos los colores";
            else if (!rojoCorrecto && !verdeCorrecto)
                feedbackText.text = "Ajusta rojo y verde";
            else if (!rojoCorrecto && !azulCorrecto)
                feedbackText.text = "Ajusta rojo y azul";
            else if (!verdeCorrecto && !azulCorrecto)
                feedbackText.text = "Ajusta verde y azul";
            else if (!rojoCorrecto)
                feedbackText.text = "Ajusta rojo";
            else if (!verdeCorrecto)
                feedbackText.text = "Ajusta verde";
            else if (!azulCorrecto)
                feedbackText.text = "Ajusta azul";
        }
    }

    private void GenerarColorObjetivo()
    {
        // Generar colores aleatorios pero no extremos (0.2 a 0.8)
        valorObjetivoRojo = Random.Range(0.2f, 0.8f);
        valorObjetivoVerde = Random.Range(0.2f, 0.8f);
        valorObjetivoAzul = Random.Range(0.2f, 0.8f);

        // Mostrar el color objetivo
        colorObjetivo.color = new Color(valorObjetivoRojo, valorObjetivoVerde, valorObjetivoAzul);

        Debug.Log($"Color objetivo - R:{valorObjetivoRojo:F2} G:{valorObjetivoVerde:F2} B:{valorObjetivoAzul:F2}");
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡CROMA PERFECTO!";

        // Efecto de destello
        float tiempo = 0;
        while (tiempo < 0.5f)
        {
            colorActual.transform.localScale = Vector3.one * (1.2f + Mathf.Sin(tiempo * 20f) * 0.1f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        CompleteMinigame();
    }

    private IEnumerator FeedbackError()
    {
        colorActual.transform.localScale = Vector3.one * 0.8f;
        yield return new WaitForSeconds(0.1f);
        colorActual.transform.localScale = Vector3.one;
    }

    public void StartMinigame()
    {
        GenerarColorObjetivo();

        enJuego = true;
        minijuegoCompletado = false;
        tiempoCorrecto = 0f;
        cuentaAtrasActivada = false;

        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.SetCanMove(false);

        panel.SetActive(true);

        // Resetear perillas a posición central
        perillaRoja.ResetearPerilla();
        perillaVerde.ResetearPerilla();
        perillaAzul.ResetearPerilla();

        // Inicializar luces
        ActualizarLuces();
        ActualizarColorMezclado();

        feedbackText.text = "Iguala el color objetivo";
        textoInstrucciones.text = "Ajusta las perillas RGB";
    }

    public void CompleteMinigame()
    {
        enJuego = false;
        playerMovement.SetCanMove(true);
        panel.SetActive(false);
        GameProgressManager.Instance.CompleteMinigame();
        Debug.Log("Minijuego Croma completado");
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
        return "Ajustar Croma";
    }

    public Transform GetTransform() => transform;
    public void FailMinigame() { }
}