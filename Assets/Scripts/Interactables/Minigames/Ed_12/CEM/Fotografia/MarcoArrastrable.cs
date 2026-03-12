using UnityEngine;
using UnityEngine.EventSystems;

public class MarcoArrastrable : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler
{
    private bool puedeMoverse = false;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetPuedeMoverse(bool estado)
    {
        puedeMoverse = estado;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown - Click en el marco");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag - Comienza arrastre");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!puedeMoverse)
        {
            Debug.Log("OnDrag ignorado - puedeMoverse = false");
            return;
        }

        Debug.Log($"OnDrag ejecutándose. Delta: {eventData.delta}");

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos
        );

        rectTransform.anchoredPosition = pos;
    }
}