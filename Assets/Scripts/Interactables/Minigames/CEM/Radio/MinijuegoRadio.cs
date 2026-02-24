using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MinijuegoRadio : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Perillas")]
    [SerializeField] private PerillaRadio perilla1;
    [SerializeField] private PerillaRadio perilla2;

    [Header("Visualización")]
    [SerializeField] private Image señal;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Configuración")]
    [SerializeField] private float tolerancia = 0.1f;
    [SerializeField] private int minigameIndex = 2;
    [SerializeField] private float tiempoRequerido = 3f;

    private float valorObjetivoPerilla1;
    private float valorObjetivoPerilla2;

    private float tiempoCorrecto = 0f;
    private bool cuentaAtrasActivada = false;

    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private bool enJuego = false;
    private bool minijuegoCompletado = false;
    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!enJuego || minijuegoCompletado) return;

        VerificarSeñalEnTiempoReal();
        if (!enJuego) return;

        Debug.Log($"Perilla1: {perilla1.ValorNormalizado:F2}, Perilla2: {perilla2.ValorNormalizado:F2}");
    }

    private void VerificarSeñalEnTiempoReal()
    {
        float diferencia1 = Mathf.Abs(perilla1.ValorNormalizado - valorObjetivoPerilla1);
        float diferencia2 = Mathf.Abs(perilla2.ValorNormalizado - valorObjetivoPerilla2);

        bool perilla1Correcta = diferencia1 < tolerancia;
        bool perilla2Correcta = diferencia2 < tolerancia;
        bool todoCorrecto = perilla1Correcta && perilla2Correcta;

        if (todoCorrecto)
        {
            // Iniciar o continuar cuenta atrás
            tiempoCorrecto += Time.deltaTime;

            if (!cuentaAtrasActivada)
            {
                cuentaAtrasActivada = true;
                feedbackText.text = $"¡Sintonizado! Mantén {tiempoRequerido:F0}s";
            }

            // Actualizar texto con cuenta atrás
            float tiempoRestante = tiempoRequerido - tiempoCorrecto;
            feedbackText.text = $"Mantén la señal: {tiempoRestante:F1}s";

            // Cambiar color según progreso
            señal.color = Color.Lerp(Color.yellow, Color.green, tiempoCorrecto / tiempoRequerido);

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
                feedbackText.text = "Señal perdida...";
                StartCoroutine(FeedbackError());
            }

            tiempoCorrecto = 0f;

            // Feedback de qué perilla ajustar
            if (!perilla1Correcta && !perilla2Correcta)
                feedbackText.text = "Ajusta ambas perillas";
            else if (!perilla1Correcta)
                feedbackText.text = "Ajusta perilla izquierda";
            else if (!perilla2Correcta)
                feedbackText.text = "Ajusta perilla derecha";

            señal.color = Color.red;
        }
    }
    private void GenerarValoresAleatorios()
    {
        valorObjetivoPerilla1 = Random.Range(0.2f, 0.8f);
        valorObjetivoPerilla2 = Random.Range(0.2f, 0.8f);

        Debug.Log($"Valores secretos - Perilla1: {valorObjetivoPerilla1}, Perilla2: {valorObjetivoPerilla2}");
    }

    public void CheckSeñal()
    {
        if (!enJuego || minijuegoCompletado) return;

        
        float diferencia1 = Mathf.Abs(perilla1.ValorNormalizado - valorObjetivoPerilla1);
        float diferencia2 = Mathf.Abs(perilla2.ValorNormalizado - valorObjetivoPerilla2);

        bool correcto = diferencia1 < tolerancia && diferencia2 < tolerancia;

        if (correcto)
        {
            feedbackText.text = "¡Correcto! Mantén la posición";
        }
        else
        {
            feedbackText.text = "Sigue ajustando";
            StartCoroutine(FeedbackError());
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡SEÑAL CLARA!";
        señal.color = Color.green;

        yield return new WaitForSeconds(0.5f);

        CompleteMinigame();
    }

    private IEnumerator FeedbackError()
    {
        Color original = señal.color;
        señal.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        señal.color = original;
    }

    public void StartMinigame()
    {
        GenerarValoresAleatorios();

        enJuego = true;
        minijuegoCompletado = false;
        tiempoCorrecto = 0f; 
        cuentaAtrasActivada = false;

        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.SetCanMove(false);

        panel.SetActive(true);

        perilla1.ResetearPerilla();
        perilla2.ResetearPerilla();

        señal.color = Color.red;
        feedbackText.text = "Ajusta las perillas";
        textoInstrucciones.text = "Sintoniza la radio";

        playerControls.Gameplay.Compress.performed -= OnConfirmar;
        playerControls.Gameplay.Compress.performed += OnConfirmar;
    }

    private void OnConfirmar(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        CheckSeñal();
    }

    public void CompleteMinigame()
    {
        enJuego = false;
        playerControls.Gameplay.Compress.performed -= OnConfirmar;
        playerMovement.SetCanMove(true);
        panel.SetActive(false);
        GameProgressManager.Instance.CompleteMinigame();
        Debug.Log("Minijuego Radio completado");
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
        return "Usar Radio";
    }

    public Transform GetTransform() => transform;
    public void FailMinigame() { }
}