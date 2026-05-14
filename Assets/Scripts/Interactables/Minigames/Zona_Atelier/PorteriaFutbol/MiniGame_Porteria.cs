using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MiniGamePorteria : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoIntentos;
    [SerializeField] private TextMeshProUGUI textoFuerza;
    [SerializeField] private RectTransform barraFuerza;
    [SerializeField] private RectTransform indicadorFuerza;

    [Header("Elementos")]
    [SerializeField] private RectTransform flecha;
    [SerializeField] private GameObject balonPrefab;
    [SerializeField] private Transform puntoLanzamiento;
    [SerializeField] private RectTransform portero;
    [SerializeField] private float velocidadPortero = 2f;

    [Header("Configuración")]
    [SerializeField] private float fuerzaMin = 5f;
    [SerializeField] private float fuerzaMax = 15f;
    [SerializeField] private float velocidadOscilacion = 3f;
    [SerializeField] private float anguloMin = 20f;
    [SerializeField] private float anguloMax = 50f;
    [SerializeField] private int golesRequeridos = 2;
    [SerializeField] private int intentosMaximos = 3;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PlayerControls playerControls;
    private int goles = 0;
    private int intentos;
    private float anguloActual = 35f;
    private float fuerzaActual;
    private bool minijuegoCompletado = false;
    private int direccionPortero = 1;
    private int direccionFuerza = 1;
    private float limiteIzquierdo = -200f;
    private float limiteDerecho = 200f;
    [SerializeField] private int minigameIndex;

    private void Start()
    {
        playerControls = InputHandler.Instance.GetControls();
        InicializarJuego();
    }

    private void InicializarJuego()
    {
        goles = 0;
        intentos = intentosMaximos;
        anguloActual = 35f;
        fuerzaActual = fuerzaMin;
        direccionFuerza = 1;

        textoPuntuacion.text = $"Goles: 0/{golesRequeridos}";
        textoIntentos.text = intentos.ToString();
        if (textoFuerza != null)
            textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";
        flecha.rotation = Quaternion.Euler(0, 0, anguloActual);
        ActualizarIndicadorFuerza();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ProcesarRotacion();
        ProcesarFuerzaOscilante();
        MoverPortero();
        DetectarDisparo();
    }

    private void MoverPortero()
    {
        Vector2 pos = portero.anchoredPosition;
        pos.x += direccionPortero * velocidadPortero * Time.deltaTime;

        if (pos.x >= limiteDerecho)
        {
            pos.x = limiteDerecho;
            direccionPortero = -1;
        }
        else if (pos.x <= limiteIzquierdo)
        {
            pos.x = limiteIzquierdo;
            direccionPortero = 1;
        }

        portero.anchoredPosition = pos;
    }

    private void ProcesarRotacion()
    {
        Vector2 move = playerControls.Gameplay.Move.ReadValue<Vector2>();
        if (move.x != 0)
        {
            anguloActual += -move.x * 100f * Time.deltaTime;
            anguloActual = Mathf.Clamp(anguloActual, anguloMin, anguloMax);
            flecha.rotation = Quaternion.Euler(0, 0, anguloActual);
        }
    }

    private void ProcesarFuerzaOscilante()
    {
        fuerzaActual += direccionFuerza * velocidadOscilacion * Time.deltaTime;

        if (fuerzaActual >= fuerzaMax)
        {
            fuerzaActual = fuerzaMax;
            direccionFuerza = -1;
        }
        else if (fuerzaActual <= fuerzaMin)
        {
            fuerzaActual = fuerzaMin;
            direccionFuerza = 1;
        }

        if (textoFuerza != null)
            textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";
        ActualizarIndicadorFuerza();
    }

    private void ActualizarIndicadorFuerza()
    {
        if (barraFuerza == null || indicadorFuerza == null) return;

        float progreso = (fuerzaActual - fuerzaMin) / (fuerzaMax - fuerzaMin);
        float alturaBarra = barraFuerza.rect.height;
        float mitadAlturaIndicador = indicadorFuerza.rect.height / 2f;
        float y = Mathf.Lerp(-alturaBarra / 2 + mitadAlturaIndicador, alturaBarra / 2 - mitadAlturaIndicador, progreso);
        indicadorFuerza.anchoredPosition = new Vector2(indicadorFuerza.anchoredPosition.x, y);
    }

    private void DetectarDisparo()
    {
        if (playerControls.Gameplay.Compress.WasPressedThisFrame())
        {
            Lanzar();
        }
    }

    private void Lanzar()
    {
        if (intentos <= 0) return;

        intentos--;
        textoIntentos.text = intentos.ToString();

        GameObject balon = Instantiate(balonPrefab, puntoLanzamiento.position, Quaternion.identity);
        BalonPorteria scriptBalon = balon.GetComponent<BalonPorteria>();

        if (scriptBalon != null)
        {
            scriptBalon.Lanzar(anguloActual, fuerzaActual, portero.position.x);
        }

        if (intentos <= 0 && goles < golesRequeridos)
        {
            StartCoroutine(ReiniciarJuego());
        }
    }

    public void RegistrarGol(string zona)
    {
        goles++;
        textoPuntuacion.text = $"Goles: {goles}/{golesRequeridos}";

        if (goles >= golesRequeridos)
            StartCoroutine(Victoria());
    }

    private IEnumerator ReiniciarJuego()
    {
        yield return new WaitForSeconds(1.5f);
        InicializarJuego();
    }

    private IEnumerator Victoria()
    {
        minijuegoCompletado = true;
        yield return new WaitForSeconds(1.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinCanchaSintetica);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}