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
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoFuerza; 
    [SerializeField] private RectTransform barraFuerza;

    [Header("Elementos")]
    [SerializeField] private RectTransform flecha;
    [SerializeField] private GameObject balonPrefab;
    [SerializeField] private Transform puntoLanzamiento; 
    [SerializeField] private Aro aro;

    [Header("Configuración")]
    [SerializeField] private float fuerzaMin = 5f; 
    [SerializeField] private float fuerzaMax = 15f; 
    [SerializeField] private float velocidadCarga = 2f;
    [SerializeField] private float anguloMin = 20f; 
    [SerializeField] private float anguloMax = 50f; 
    [SerializeField] private float velocidadRotacion = 100f; 
    [SerializeField] private int canastasRequeridas = 3;
    [SerializeField] private int intentosMaximos = 6;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PlayerControls playerControls;
    private int canastas = 0;
    private int intentos;
    private float anguloActual = 35f;
    private float fuerzaActual = 5f;
    private bool cargando = false;
    private bool minijuegoCompletado = false;

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

        textoPuntuacion.text = $"0/{canastasRequeridas}";
        textoIntentos.text = intentos.ToString();
        textoInstrucciones.text = "← →: Ángulo | ESPACIO: Fuerza";
        if (textoFuerza != null)
            textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";
        flecha.rotation = Quaternion.Euler(0, 0, anguloActual);

        ActualizarBarraFuerza();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ProcesarRotacion();
        ProcesarFuerza();
    }

    private void ProcesarRotacion()
    {
        Vector2 move = playerControls.Gameplay.Move.ReadValue<Vector2>();
        if (move.x != 0)
        {
            anguloActual += move.x * velocidadRotacion * Time.deltaTime;
            anguloActual = Mathf.Clamp(anguloActual, anguloMin, anguloMax);
            flecha.rotation = Quaternion.Euler(0, 0, anguloActual);
        }
    }

    private void ProcesarFuerza()
    {
        if (playerControls.Gameplay.Compress.WasPressedThisFrame() && !cargando)
        {
            cargando = true;
            fuerzaActual = fuerzaMin;
        }

        if (cargando)
        {
            fuerzaActual += (fuerzaMax - fuerzaMin) / 1.5f * Time.deltaTime;
            fuerzaActual = Mathf.Clamp(fuerzaActual, fuerzaMin, fuerzaMax);
            ActualizarBarraFuerza();

            if (textoFuerza != null)
                textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";
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
        Balon scriptBalon = balon.GetComponent<Balon>();

        if (scriptBalon != null)
        {
            float angulo = flecha.rotation.eulerAngles.z;
            scriptBalon.Lanzar(angulo, fuerzaActual);
        }

        if (intentos <= 0 && canastas < canastasRequeridas)
        {
            StartCoroutine(ReiniciarJuego());
        }
    }

    private IEnumerator ReiniciarJuego()
    {
        textoInstrucciones.text = "¡Sin intentos! Reiniciando...";
        yield return new WaitForSeconds(1.5f);
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
        textoInstrucciones.text = "¡VICTORIA!";
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}