using UnityEngine;
using UnityEngine.EventSystems;

public class MarcoArrastrable : MonoBehaviour, IDragHandler
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

    public void OnDrag(PointerEventData eventData)
    {
        if (!puedeMoverse) return;

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