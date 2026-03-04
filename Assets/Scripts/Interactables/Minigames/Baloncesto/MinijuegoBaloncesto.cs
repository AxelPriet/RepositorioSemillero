using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class MinijuegoBaloncesto : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoIntentos;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoFuerza;

    [Header("Elementos")]
    [SerializeField] private RectTransform flecha;
    [SerializeField] private GameObject balonPrefab; 
    [SerializeField] private Aro aro;

    [Header("Configuración")]
    [SerializeField] private float fuerzaMin = 15f;
    [SerializeField] private float fuerzaMax = 40f;
    [SerializeField] private float tiempoCarga = 1.5f;
    [SerializeField] private int canastasRequeridas = 3;
    [SerializeField] private int intentosMaximos = 6;
    [SerializeField] private int minigameIndex = 5;

    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private bool enJuego = false;
    private int canastas = 0;
    private int intentos;
    private bool cargando = false;
    private float fuerzaActual = 5f;
    private GameObject balonActual; 

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!enJuego) return;
        if (playerControls == null) return;

        // Rotar flecha
        Vector2 move = playerControls.Gameplay.Move.ReadValue<Vector2>();
        if (move.x != 0)
        {
            float angulo = flecha.rotation.eulerAngles.z;
            angulo -= move.x * 100f * Time.deltaTime;
            angulo = Mathf.Clamp(angulo, 20, 70);
            flecha.rotation = Quaternion.Euler(0, 0, angulo);
        }

        // Cargar fuerza con espacio
        if (playerControls.Gameplay.Compress.WasPressedThisFrame() && !cargando)
        {
            cargando = true;
            fuerzaActual = fuerzaMin;
        }

        if (playerControls.Gameplay.Compress.IsPressed() && cargando)
        {
            fuerzaActual += (fuerzaMax - fuerzaMin) / tiempoCarga * Time.deltaTime;
            fuerzaActual = Mathf.Clamp(fuerzaActual, fuerzaMin, fuerzaMax);

            if (textoFuerza != null)
                textoFuerza.text = $"Fuerza: {fuerzaActual:F1}";
        }

        if (playerControls.Gameplay.Compress.WasReleasedThisFrame() && cargando)
        {
            cargando = false;
            Lanzar();

            if (textoFuerza != null)
                textoFuerza.text = "Fuerza: 5.0";
        }
    }

    private void Lanzar()
    {
        if (intentos <= 0) return;

        intentos--;
        textoIntentos.text = intentos.ToString();

        // INSTANCIAR NUEVO BALÓN
        balonActual = Instantiate(balonPrefab, flecha.position, Quaternion.identity, panel.transform);

        float angulo = flecha.rotation.eulerAngles.z;
        Balon scriptBalon = balonActual.GetComponent<Balon>();

        if (scriptBalon != null)
        {
            scriptBalon.Lanzar(angulo, fuerzaActual);
            Debug.Log($"Balón instanciado - Ángulo: {angulo}, Fuerza: {fuerzaActual}"); // ← PARA VERIFICAR
        }
        else
        {
            Debug.LogError("El prefab del balón no tiene el script Balon.cs");
        }
    }

    private IEnumerator Victoria()
    {
        textoInstrucciones.text = "¡VICTORIA!";
        yield return new WaitForSeconds(1f);
        CompleteMinigame();
    }
    public void RegistrarCanasta()
    {
        canastas++;
        textoPuntuacion.text = $"{canastas}/{canastasRequeridas}";

        Debug.Log($"¡Canasta! Total: {canastas}");

        if (canastas >= canastasRequeridas)
        {
            StartCoroutine(Victoria());
        }
    }

    public void StartMinigame()
    {
        enJuego = true;
        canastas = 0;
        intentos = intentosMaximos;
        cargando = false;
        fuerzaActual = fuerzaMin;

        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        panel.SetActive(true);

        textoPuntuacion.text = $"0/{canastasRequeridas}";
        textoIntentos.text = intentos.ToString();
        textoInstrucciones.text = "← →: Ángulo | ESPACIO: Cargar y soltar";
        textoFuerza.text = "Fuerza: 5.0";

        flecha.rotation = Quaternion.Euler(0, 0, 45);
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
        //return GameProgressManager.Instance.CanPlayMinigame(minigameIndex) && !enJuego;
        return !enJuego;
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