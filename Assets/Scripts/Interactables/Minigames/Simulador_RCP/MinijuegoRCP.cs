using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinijuegoRCP : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("UI")]
    public GameObject panel;
    public RectTransform indicador;
    public RectTransform zonaVerde;
    [SerializeField] private TMPro.TextMeshProUGUI textoPuntos;
    [SerializeField] private TMPro.TextMeshProUGUI textoErrores;

    [Header("Configuración")]
    public float velocidad = 200f;
    public int puntosNecesarios = 5;

    private float direccion = 1f;
    private float minY;
    private float maxY;
    private int puntos = 0;
    private int errores = 0;

    private bool enPausa = false;
    [SerializeField] private float tiempoPausa = 0.3f;


    private InputAction compressAction;
    [SerializeField] private int minigameIndex = 0;
    private InputHandler inputHandler;
    private PlayerControls playerControls;


    void Start()
    {
        float alturaBarra = ((RectTransform)indicador.parent).rect.height;

        minY = -alturaBarra / 2 + (indicador.rect.height / 2);
        maxY = alturaBarra / 2 - (indicador.rect.height / 2);

        panel.SetActive(false);
    }
    private void Awake()
    {
        inputHandler = FindObjectOfType<InputHandler>();
    }
    void Update()
    {
        if (panel.activeSelf)
            MoverIndicador();
    }
    private void ActualizarUI()
    {
        textoPuntos.text = "Aciertos: " + puntos;
        textoErrores.text = "Errores: " + errores;
    }


    public void Interactuar()
    {
        MinigameManager.Instance.StartMinigame(this, minigameIndex);
    }

    public string GetPrompt()
    {
        return "Iniciar RCP";
    }

    public bool PuedeInteractuar()
    {
        return true; 
    }

    public Transform GetTransform()
    {
        return transform;
    }


    public void StartMinigame()
    {
        if (inputHandler == null)
            inputHandler = FindObjectOfType<InputHandler>();

        if (playerControls == null && inputHandler != null)

        if (playerControls == null)
        {
            Debug.LogError("PlayerControls es NULL");
            return;
        }

        panel.SetActive(true);
        puntos = 0;

        errores = 0;
        ActualizarUI();

        playerControls.Gameplay.Enable();
        playerControls.Gameplay.Move.Disable();
        playerControls.Gameplay.Compress.performed += OnCompress;
    }


    public void CompleteMinigame()
    {
        playerControls.Gameplay.Compress.performed -= OnCompress;

        playerControls.Gameplay.Move.Enable();


        panel.SetActive(false);
    }

    public void FailMinigame()
    {
        playerControls.Gameplay.Compress.performed -= OnCompress;

        playerControls.Gameplay.Move.Enable();


        panel.SetActive(false);
    }


    private void OnCompress(InputAction.CallbackContext context)
    {
        VerificarZona();
    }

    void VerificarZona()
    {
        float indicadorY = indicador.anchoredPosition.y;

        float zonaMin = zonaVerde.anchoredPosition.y - (zonaVerde.rect.height / 2);
        float zonaMax = zonaVerde.anchoredPosition.y + (zonaVerde.rect.height / 2);

        if (indicadorY >= zonaMin && indicadorY <= zonaMax)
        {
            puntos++;
            Debug.Log("¡Buena compresión! Puntos: " + puntos);

            StartCoroutine(PausaIndicador());
            ActualizarUI();

            if (puntos >= puntosNecesarios)
            {
                CompleteMinigame();
            }
        }
        else
        {
            errores++;
            Debug.Log("Compresión incorrecta");

            ActualizarUI();

            if (errores >= 3)
            {
                FailMinigame();
            }
        }
    }


    void MoverIndicador()
    {
        if (enPausa) return;
        Vector2 pos = indicador.anchoredPosition;
        pos.y += direccion * velocidad * Time.deltaTime;

        if (pos.y >= maxY)
        {
            pos.y = maxY;
            direccion = -1;
        }
        else if (pos.y <= minY)
        {
            pos.y = minY;
            direccion = 1;
        }

        indicador.anchoredPosition = pos;
    }
    private IEnumerator PausaIndicador()
    {
        enPausa = true;
        yield return new WaitForSeconds(tiempoPausa);
        enPausa = false;
    }
}