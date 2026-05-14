using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinijuegoBaloncesto : MonoBehaviour
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
    [SerializeField] private Aro aro;

    [Header("Configuración")]
    [SerializeField] private float fuerzaMin = 5f;
    [SerializeField] private float fuerzaMax = 15f;
    [SerializeField] private float velocidadOscilacion = 2f; 
    [SerializeField] private float anguloMin = 20f;
    [SerializeField] private float anguloMax = 60f;
    [SerializeField] private float velocidadRotacion = 100f;
    [SerializeField] private int canastasRequeridas = 3;
    [SerializeField] private int intentosMaximos = 6;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PlayerControls playerControls;
    private int canastas = 0;
    private int intentos;
    private float anguloActual = 35f;
    private float fuerzaActual = 5f;
    private bool minijuegoCompletado = false;
    private int direccionFuerza = 1;  
    private GameObject balonActual;
    [SerializeField] private int minigameIndex;

    private void Start()
    {
        playerControls = InputHandler.Instance.GetControls();
        InicializarJuego();
    }

    private void InicializarJuego()
    {
        canastas = 0;
        intentos = intentosMaximos;
        anguloActual = 35f;
        fuerzaActual = fuerzaMin;
        direccionFuerza = 1;

        textoPuntuacion.text = $"0/{canastasRequeridas}";
        textoIntentos.text = intentos.ToString();
        textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";

        flecha.rotation = Quaternion.Euler(0, 0, anguloActual);
        ActualizarBarraFuerza();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ProcesarRotacion();
        ProcesarFuerzaOscilante();  
        DetectarDisparo();
    }

    private void ProcesarRotacion()
    {
        Vector2 move = playerControls.Gameplay.Move.ReadValue<Vector2>();
        if (move.x != 0)
        {
            anguloActual += -move.x * velocidadRotacion * Time.deltaTime;
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

        textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";
        ActualizarBarraFuerza();
    }

    private void ActualizarBarraFuerza()
    {
        if (barraFuerza != null && indicadorFuerza != null)
        {
            float alturaBarra = barraFuerza.rect.height;
            float progreso = (fuerzaActual - fuerzaMin) / (fuerzaMax - fuerzaMin);
            float y = Mathf.Lerp(-alturaBarra / 2f, alturaBarra / 2f, progreso);
            indicadorFuerza.localPosition = new Vector3(indicadorFuerza.localPosition.x, y, indicadorFuerza.localPosition.z);
        }
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

        balonActual = Instantiate(balonPrefab, puntoLanzamiento.position, Quaternion.identity);
        Balon scriptBalon = balonActual.GetComponent<Balon>();

        if (scriptBalon != null)
        {
            float angulo = flecha.rotation.eulerAngles.z;
            scriptBalon.Lanzar(angulo, fuerzaActual);
        }

        if (intentos <= 0 && canastas < canastasRequeridas)
        {
            StartCoroutine(EsperarBalonYReiniciar());
        }
    }

    private IEnumerator EsperarBalonYReiniciar()
    {
        while (balonActual != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        InicializarJuego();
    }

    public void RegistrarCanasta()
    {
        canastas++;
        textoPuntuacion.text = $"{canastas}/{canastasRequeridas}";
        if (canastas >= canastasRequeridas)
        {
            StartCoroutine(Victoria());
        }
    }

    private IEnumerator Victoria()
    {
        minijuegoCompletado = true;
        yield return new WaitForSeconds(1.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinBasket);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}