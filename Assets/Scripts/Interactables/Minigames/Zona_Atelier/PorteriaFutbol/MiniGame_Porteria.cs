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
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private RectTransform barraFuerza;

    [Header("Elementos")]
    [SerializeField] private RectTransform flecha;
    [SerializeField] private GameObject balonPrefab;
    [SerializeField] private Transform puntoLanzamiento;
    [SerializeField] private RectTransform portero;
    [SerializeField] private float velocidadPortero = 2f;

    [Header("Configuración")]
    [SerializeField] private float fuerzaMin = 5f;
    [SerializeField] private float fuerzaMax = 15f;
    [SerializeField] private float velocidadCarga = 3f;
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
    private bool cargando = false;
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

        textoPuntuacion.text = $"Goles: 0/{golesRequeridos}";
        textoIntentos.text = intentos.ToString();
        textoInstrucciones.text = "← →: Ángulo | ESPACIO: Fuerza";
        flecha.rotation = Quaternion.Euler(0, 0, anguloActual);
        ActualizarBarraFuerza();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ProcesarRotacion();
        ProcesarFuerza();
        MoverPortero();
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
            anguloActual += move.x * 100f * Time.deltaTime;
            anguloActual = Mathf.Clamp(anguloActual, anguloMin, anguloMax);
            flecha.rotation = Quaternion.Euler(0, 0, anguloActual);
        }
    }

    private void ProcesarFuerza()
    {
        if (playerControls.Gameplay.Compress.WasPressedThisFrame() && !cargando)
        {
            cargando = true;
            direccionFuerza = 1;
            fuerzaActual = fuerzaMin;
        }

        if (cargando)
        {
            fuerzaActual += direccionFuerza * velocidadCarga * Time.deltaTime;

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

            ActualizarBarraFuerza();
        }

        if (playerControls.Gameplay.Compress.WasReleasedThisFrame() && cargando)
        {
            cargando = false;
            Lanzar();
        }
    }

    private void ActualizarBarraFuerza()
    {
        float progreso = (fuerzaActual - fuerzaMin) / (fuerzaMax - fuerzaMin);
        barraFuerza.localScale = new Vector3(progreso, 1, 1);
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

        string mensaje = zona == "centro" ? "¡Gol al centro!" :
                        zona == "izquierda" ? "¡Gol a la izquierda!" : "¡Gol a la derecha!";
        textoInstrucciones.text = mensaje;

        if (goles >= golesRequeridos)
            StartCoroutine(Victoria());
    }

    private IEnumerator ReiniciarJuego()
    {
        textoInstrucciones.text = "¡Sin intentos! Reiniciando...";
        yield return new WaitForSeconds(1.5f);
        InicializarJuego();
    }

    private IEnumerator Victoria()
    {
        minijuegoCompletado = true;
        textoInstrucciones.text = "¡VICTORIA!";
        yield return new WaitForSeconds(1.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinCanchaSintetica);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}