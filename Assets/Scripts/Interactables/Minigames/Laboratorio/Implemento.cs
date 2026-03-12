using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Implemento : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private string nombreImplemento;
    [SerializeField] private float distanciaSnap = 50f;
    [SerializeField] private int tamaño;

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
    public int Tamaño
    {
        get { return tamaño; }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocado) return;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        if (slotActual != null)
        {
            slotActual.Liberar();
            slotActual = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocado) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocado) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        SlotEstante slotCercano = BuscarSlotCercano();

        if (slotCercano != null && slotCercano.EspacioDisponible(tamaño))
        {
            // Colocar en el slot
            rectTransform.position = slotCercano.transform.position;
            slotCercano.Ocupar(tamaño, this);
            slotActual = slotCercano;
            colocado = true;
            canvasGroup.blocksRaycasts = false;

            FindFirstObjectByType<MiniGame_Inventario>()?.ImplementoColocado();
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
        colocado = false;
        rectTransform.anchoredPosition = posicionInicial;
        canvasGroup.blocksRaycasts = true;

        if (slotActual != null)
        {
            slotActual.Liberar();
            slotActual = null;
        }
    }
}