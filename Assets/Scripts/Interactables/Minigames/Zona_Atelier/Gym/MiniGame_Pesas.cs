using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MiniGame_Pesas : MonoBehaviour
{
    [Header("Elementos UI")]
    [SerializeField] private Slider barraFuerza;
    [SerializeField] private RectTransform pesa;
    [SerializeField] private TextMeshProUGUI textoTemporizador;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoResultado;

    [Header("Configuración")]
    [SerializeField] private float tiempoLimite = 10f;
    [SerializeField] private float incrementoPorClick = 0.05f;
    [SerializeField] private float alturaMaximaPesa = 200f;
    [SerializeField] private float decrementoNatural = 0.1f;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private float tiempoRestante;
    private float progresoActual = 0f;
    private float pesaPosicionInicial;
    private bool juegoActivo = false;
    private bool temporizadorActivo = false;
    private bool juegoTerminado = false;
    private bool primerClick = false;
    private PlayerControls playerControls;

    private void Start()
    {
        playerControls = InputHandler.Instance.GetControls();
        InicializarJuego();
    }

    private void InicializarJuego()
    {
        tiempoRestante = tiempoLimite;
        progresoActual = 0f;
        juegoActivo = true;
        temporizadorActivo = false;
        juegoTerminado = false;
        primerClick = false;

        if (pesa != null)
            pesaPosicionInicial = pesa.anchoredPosition.y;

        ActualizarUI();

        playerControls.Gameplay.Compress.performed += OnPresionarEspacio;
    }

    private void Update()
    {
        if (!juegoActivo || juegoTerminado) return;

        if (temporizadorActivo)
        {
            tiempoRestante -= Time.deltaTime;
            textoTemporizador.text = $"Tiempo: {tiempoRestante:F1}s";

            if (!Keyboard.current.spaceKey.isPressed)
            {
                progresoActual -= decrementoNatural * Time.deltaTime;
                progresoActual = Mathf.Max(0, progresoActual);
                ActualizarVisuales();
            }

            if (tiempoRestante <= 0)
            {
                StartCoroutine(PerderJuego());
            }
        }
        else
        {
            textoTemporizador.text = "Presiona ESPACIO para comenzar";
        }
    }

    private void OnPresionarEspacio(InputAction.CallbackContext context)
    {
        if (!juegoActivo || juegoTerminado) return;

        if (!primerClick)
        {
            primerClick = true;
            temporizadorActivo = true;
            textoInstrucciones.text = "¡Sigue presionando!";
        }

        progresoActual += incrementoPorClick;
        progresoActual = Mathf.Min(1f, progresoActual);

        ActualizarVisuales();

        if (progresoActual >= 1f)
        {
            StartCoroutine(GanarJuego());
        }
    }

    private void ActualizarVisuales()
    {
        if (barraFuerza != null)
            barraFuerza.value = progresoActual;

        if (pesa != null)
        {
            float nuevaY = pesaPosicionInicial + (progresoActual * alturaMaximaPesa);
            pesa.anchoredPosition = new Vector2(pesa.anchoredPosition.x, nuevaY);
        }
    }

    private void ActualizarUI()
    {
        barraFuerza.value = 0;
        textoTemporizador.text = "Presiona ESPACIO para comenzar";
        textoInstrucciones.text = "Presiona ESPACIO rápidamente";
        textoResultado.gameObject.SetActive(false);
    }

    private IEnumerator GanarJuego()
    {
        juegoActivo = false;
        juegoTerminado = true;

        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡LEVANTASTE LA PESA!";
        textoResultado.color = Color.green;

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private IEnumerator PerderJuego()
    {
        juegoActivo = false;
        juegoTerminado = true;

        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡TIEMPO AGOTADO!";
        textoResultado.color = Color.red;

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (playerControls != null)
            playerControls.Gameplay.Compress.performed -= OnPresionarEspacio;
    }
}