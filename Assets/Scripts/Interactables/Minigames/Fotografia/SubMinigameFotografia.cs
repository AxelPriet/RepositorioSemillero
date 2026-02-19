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

    [SerializeField] private TMPro.TextMeshProUGUI textoInstrucciones;

    // ISubMinigame
    public void StartSubMinigame(MinigameCombinado parent)
    {
        this.parent = parent;
        gameObject.SetActive(true);
        puedeTomarFoto = true;
        textoInstrucciones.text = "Encuadra al objetivo dentro del marco y presiona la tecla espacio para tomar la foto";
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
        if (!puedeTomarFoto) return;

    }
    private void Awake()
    {
        playerControls = InputHandler.Instance.GetControls();
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
            FailSubMinigame();
        }
    }

    // Arrastrar el marco con mouse
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