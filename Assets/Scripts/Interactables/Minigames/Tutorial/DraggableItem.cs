using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public string itemID;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 posicionOriginal;
    private Transform padreOriginal;
    private Canvas canvas;
    private bool colocadoCorrectamente = false; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocadoCorrectamente) return; 
        posicionOriginal = rectTransform.anchoredPosition;
        padreOriginal = transform.parent;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocadoCorrectamente) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (!colocadoCorrectamente) 
        {
            transform.SetParent(padreOriginal);
            rectTransform.anchoredPosition = posicionOriginal;
        }
    }

    public void ColocarEnSilueta(Transform silueta)
    {
        colocadoCorrectamente = true; 
        transform.SetParent(silueta);
        rectTransform.anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = false;
    }
}