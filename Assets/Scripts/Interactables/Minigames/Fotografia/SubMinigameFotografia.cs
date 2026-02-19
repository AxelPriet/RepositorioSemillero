using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SubMinijuegoFotografia : MonoBehaviour, ISubMinigame, IPointerDownHandler, IDragHandler
{
    [Header("UI del Fotograma")]
    public RectTransform marco; 
    public Canvas canvas; 

    [Header("Objetivo")]
    public Transform objetivo; 

    [Header("Configuración")]
    private PlayerControls playerControls;

    private MinigameCombinado parent; 
    private bool puedeTomarFoto = false;

    private Vector2 marcoPosInicial;

    [SerializeField] private TMPro.TextMeshProUGUI textoInstrucciones;

    // ISubMinigame
    public void StartSubMinigame(MinigameCombinado parent)
    {
        this.parent = parent;
        gameObject.SetActive(true);

        puedeTomarFoto = true;
        marco.localPosition = marcoPosInicial;
        textoInstrucciones.text = "Encuadra al objetivo dentro del marco y presiona la tecla espacio para tomar la foto";

        playerControls = InputHandler.Instance.GetControls();
        playerControls.Gameplay.Compress.performed -= OnTomarFoto;
        playerControls.Gameplay.Compress.performed += OnTomarFoto;
    }



    public void CompleteSubMinigame()
    {
        gameObject.SetActive(false);
        parent.OnSubMinijuegoComplete();
        playerControls.Gameplay.Compress.performed -= OnTomarFoto;
    }

    public void FailSubMinigame()
    {
        gameObject.SetActive(false);
        parent.OnSubMinijuegoFail();
        playerControls.Gameplay.Compress.performed -= OnTomarFoto;
    }

    // Actualización
    private void Update()
    {

    }
    private void Awake()
    {
        playerControls = InputHandler.Instance.GetControls();
        marcoPosInicial = marco.localPosition;
    }
    private void OnTomarFoto(InputAction.CallbackContext context)
    {
        Vector2 objetivoScreenPos = Camera.main.WorldToScreenPoint(objetivo.position);

        if (RectTransformUtility.RectangleContainsScreenPoint(marco, objetivoScreenPos, canvas.worldCamera))
        {
            CompleteSubMinigame();
        }
        else
        {
            marco.localPosition = marcoPosInicial;

            textoInstrucciones.text = "¡Intenta de nuevo! Ajusta el marco sobre el objetivo y presiona espacio";

            puedeTomarFoto = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // IPointerDownHandler
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos
        );
        marco.localPosition = pos;
    }
}