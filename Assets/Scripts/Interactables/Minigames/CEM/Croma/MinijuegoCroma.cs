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
    [SerializeField] private float tiempoRequerido = 2f;

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

        ActualizarLuces();
        ActualizarColorMezclado();
        VerificarMezclaEnTiempoReal();
    }

    private void ActualizarLuces()
    {
        Color colorRojo = Color.red * perillaRoja.ValorNormalizado;
        Color colorVerde = Color.green * perillaVerde.ValorNormalizado;
        Color colorAzul = Color.blue * perillaAzul.ValorNormalizado;

        luzRoja.color = new Color(colorRojo.r, colorRojo.g, colorRojo.b, 1f);
        luzVerde.color = new Color(colorVerde.r, colorVerde.g, colorVerde.b, 1f);
        luzAzul.color = new Color(colorAzul.r, colorAzul.g, colorAzul.b, 1f);
    }

    private void ActualizarColorMezclado()
    {
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
            tiempoCorrecto += Time.deltaTime;

            if (!cuentaAtrasActivada)
            {
                cuentaAtrasActivada = true;
            }

            float tiempoRestante = tiempoRequerido - tiempoCorrecto;
            feedbackText.text = $"¡Color perfecto! Mantén {tiempoRestante:F1}s";
            colorActual.transform.localScale = Vector3.one * (1f + (tiempoCorrecto / tiempoRequerido * 0.2f));

            if (tiempoCorrecto >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            if (cuentaAtrasActivada)
            {
                cuentaAtrasActivada = false;
                StartCoroutine(FeedbackError());
            }

            tiempoCorrecto = 0f;
            colorActual.transform.localScale = Vector3.one;

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
        valorObjetivoRojo = Random.Range(0.2f, 0.8f);
        valorObjetivoVerde = Random.Range(0.2f, 0.8f);
        valorObjetivoAzul = Random.Range(0.2f, 0.8f);
        colorObjetivo.color = new Color(valorObjetivoRojo, valorObjetivoVerde, valorObjetivoAzul);
        Debug.Log($"Color objetivo - R:{valorObjetivoRojo:F2} G:{valorObjetivoVerde:F2} B:{valorObjetivoAzul:F2}");
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡CROMA PERFECTO!";

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
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        panel.SetActive(true);

        perillaRoja.ResetearPerilla();
        perillaVerde.ResetearPerilla();
        perillaAzul.ResetearPerilla();

        ActualizarLuces();
        ActualizarColorMezclado();

        feedbackText.text = "Iguala el color objetivo";
        textoInstrucciones.text = "Ajusta las perillas RGB";
    }

    public void CompleteMinigame()
    {
        enJuego = false;

        if (playerMovement != null)
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