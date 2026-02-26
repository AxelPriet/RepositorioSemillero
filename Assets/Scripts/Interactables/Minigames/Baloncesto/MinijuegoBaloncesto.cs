using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class MinijuegoBaloncesto : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoIntentos;

    [Header("Controladores")]
    [SerializeField] private ControladorAngulo controladorAngulo;
    [SerializeField] private ControladorFuerza controladorFuerza;
    [SerializeField] private ControladorTrayectoria controladorTrayectoria;
    [SerializeField] private ControladorBalon controladorBalon;
    [SerializeField] private DetectorCanasta detectorCanasta;

    [Header("Configuración")]
    [SerializeField] private int canastasRequeridas = 3;
    [SerializeField] private int intentosMaximos = 6;
    [SerializeField] private int minigameIndex = 5;

    // Estado del juego
    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private bool enJuego = false;
    private bool minijuegoCompletado = false;

    private int canastasActuales = 0;
    private int intentosRestantes;

    private enum EstadoLanzamiento { AjustandoAngulo, AjustandoFuerza, Lanzado }
    private EstadoLanzamiento estadoActual = EstadoLanzamiento.AjustandoAngulo;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!enJuego || minijuegoCompletado) return;

        switch (estadoActual)
        {
            case EstadoLanzamiento.AjustandoAngulo:
                controladorAngulo.Actualizar();
                controladorTrayectoria.ActualizarPuntos(
                    controladorAngulo.ObtenerAngulo(),
                    controladorFuerza.ObtenerFuerzaActual()
                );
                break;

            case EstadoLanzamiento.AjustandoFuerza:
                controladorFuerza.Actualizar();
                controladorTrayectoria.ActualizarPuntos(
                    controladorAngulo.ObtenerAngulo(),
                    controladorFuerza.ObtenerFuerzaActual()
                );
                break;

            case EstadoLanzamiento.Lanzado:
                controladorTrayectoria.OcultarPuntos();
                break;
        }

        // Click izquierdo para cambiar de estado
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ProcesarClick();
        }
    }

    private void ProcesarClick()
    {
        switch (estadoActual)
        {
            case EstadoLanzamiento.AjustandoAngulo:
                estadoActual = EstadoLanzamiento.AjustandoFuerza;
                controladorFuerza.IniciarMovimiento();
                textoInstrucciones.text = "Click para lanzar";
                break;

            case EstadoLanzamiento.AjustandoFuerza:
                estadoActual = EstadoLanzamiento.Lanzado;
                LanzarBalon();
                break;
        }
    }

    private void LanzarBalon()
    {
        if (intentosRestantes <= 0) return;

        intentosRestantes--;
        textoIntentos.text = $"{intentosRestantes}";

        controladorBalon.Lanzar(
            controladorAngulo.ObtenerAngulo(),
            controladorFuerza.ObtenerFuerzaActual()
        );

        StartCoroutine(EsperarResultado());
    }

    private IEnumerator EsperarResultado()
    {
        yield return new WaitForSeconds(3f);

        if (intentosRestantes <= 0 && canastasActuales < canastasRequeridas)
        {
            ReiniciarMinijuego();
        }
        else
        {
            estadoActual = EstadoLanzamiento.AjustandoAngulo;
            controladorFuerza.Resetear();
            textoInstrucciones.text = "Ajusta ángulo con el mouse";
        }
    }

    public void RegistrarCanasta()
    {
        if (!enJuego || minijuegoCompletado) return;

        canastasActuales++;
        textoPuntuacion.text = $"{canastasActuales}/{canastasRequeridas}";

        if (canastasActuales >= canastasRequeridas)
        {
            StartCoroutine(CompletarMinijuego());
        }
    }

    private void ReiniciarMinijuego()
    {
        canastasActuales = 0;
        intentosRestantes = intentosMaximos;
        estadoActual = EstadoLanzamiento.AjustandoAngulo;
        controladorFuerza.Resetear();

        textoPuntuacion.text = $"0/{canastasRequeridas}";
        textoIntentos.text = $"{intentosRestantes}";
        textoInstrucciones.text = "Ajusta ángulo con el mouse";
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        textoInstrucciones.text = "¡VICTORIA!";
        yield return new WaitForSeconds(1f);
        CompleteMinigame();
    }

    public void StartMinigame()
    {
        enJuego = true;
        minijuegoCompletado = false;
        canastasActuales = 0;
        intentosRestantes = intentosMaximos;
        estadoActual = EstadoLanzamiento.AjustandoAngulo;

        playerControls = InputHandler.Instance?.GetControls();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        panel.SetActive(true);

        textoPuntuacion.text = $"0/{canastasRequeridas}";
        textoIntentos.text = $"{intentosRestantes}";
        textoInstrucciones.text = "Ajusta ángulo con el mouse";

        controladorAngulo.Resetear();
        controladorFuerza.Resetear();
        controladorTrayectoria.CrearPuntos(panel.transform);
        detectorCanasta.SetMinijuego(this);
    }

    public void CompleteMinigame()
    {
        enJuego = false;

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        panel.SetActive(false);
        GameProgressManager.Instance.CompleteMinigame();
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
        return "Jugar al Baloncesto";
    }

    public Transform GetTransform() => transform;
    public void FailMinigame() { }
}