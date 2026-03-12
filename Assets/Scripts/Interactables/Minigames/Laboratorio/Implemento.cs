using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Implemento : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private string nombreImplemento;
    [SerializeField] private float distanciaSnap = 50f;
    [SerializeField] private int tamaño;

    [Header("Hover")]
    [SerializeField] private GameObject panelHoverPrefab;

    private GameObject panelHoverActual;
    private RectTransform canvasRect;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posicionInicial;
    private CanvasGroup canvasGroup;
    private bool colocado = false;
    private Camera camara;
    private Image imagen;
    private SlotEstante slotActual = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        camara = Camera.main;
        imagen = GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        posicionInicial = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (panelHoverPrefab != null && panelHoverActual == null)
        {
            panelHoverActual = Instantiate(panelHoverPrefab, canvasRect);

            TextMeshProUGUI texto = panelHoverActual.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null)
                texto.text = $"{nombreImplemento}\nTamaño: {tamaño}";

            ActualizarPosicionPanel(eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelHoverActual != null)
        {
            Destroy(panelHoverActual);
            panelHoverActual = null;
        }
    }

    private void Update()
    {
        if (panelHoverActual != null)
        {
            ActualizarPosicionPanel(Mouse.current.position.ReadValue());
        }
    }

    private void ActualizarPosicionPanel(Vector2 posicionMouse)
    {
        if (panelHoverActual != null && canvasRect != null)
        {
            RectTransform panelRect = panelHoverActual.GetComponent<RectTransform>();

            Vector2 posLocal;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                posicionMouse + new Vector2(20, 20),
                Camera.main,
                out posLocal
            );

            panelRect.anchoredPosition = posLocal;
        }
    }

    public int Tamaño
    {
        get { return tamaño; }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        // Si estaba en un slot, removerlo
        if (slotActual != null)
        {
            slotActual.RemoverImplemento(this);
            slotActual = null;
            colocado = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        SlotEstante slotCercano = BuscarSlotCercano();

        if (slotCercano != null && slotCercano.EspacioDisponible(tamaño))
        {
            // Colocar en el slot
            rectTransform.position = slotCercano.transform.position;
            slotCercano.AgregarImplemento(this);
            slotActual = slotCercano;
            colocado = true;
        }
        else
        {
            StartCoroutine(FeedbackError());
            RegresarInicio();
        }
    }

    private SlotEstante BuscarSlotCercano()
    {
        SlotEstante[] todosLosSlots = FindObjectsByType<SlotEstante>(FindObjectsSortMode.None);
        SlotEstante masCercano = null;
        float distanciaMinima = distanciaSnap;

        foreach (var slot in todosLosSlots)
        {
            float distancia = Vector2.Distance(rectTransform.position, slot.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = slot;
            }
        }

        return masCercano;
    }

    private void RegresarInicio()
    {
        rectTransform.anchoredPosition = posicionInicial;
    }

    private IEnumerator FeedbackError()
    {
        Color colorOriginal = imagen.color;
        imagen.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        imagen.color = colorOriginal;
    }

    public void Resetear()
    {
        if (slotActual != null)
        {
            slotActual.RemoverImplemento(this);
            slotActual = null;
        }
        colocado = false;
        rectTransform.anchoredPosition = posicionInicial;
        canvasGroup.blocksRaycasts = true;
    }
}