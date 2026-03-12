using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ElementoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración")]
    [SerializeField] private GameObject panelInfoPrefab; 
    [SerializeField] private string textoInfo = "Tamaño: 2";
    [SerializeField] private Vector2 offset = new Vector2(20, 20);

    private GameObject panelInfoActual;
    private RectTransform canvasRect;

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (panelInfoPrefab != null && panelInfoActual == null)
        {
            panelInfoActual = Instantiate(panelInfoPrefab, canvasRect);
            ActualizarPosicionPanel(eventData.position);

            TextMeshProUGUI texto = panelInfoActual.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null)
                texto.text = textoInfo;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelInfoActual != null)
        {
            Destroy(panelInfoActual);
            panelInfoActual = null;
        }
    }

    private void Update()
    {
        if (panelInfoActual != null)
        {
            ActualizarPosicionPanel(Mouse.current.position.ReadValue());
        }
    }

    private void ActualizarPosicionPanel(Vector2 posicionMouse)
    {
        if (panelInfoActual != null && canvasRect != null)
        {
            RectTransform panelRect = panelInfoActual.GetComponent<RectTransform>();

            Vector2 posLocal;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                posicionMouse + offset,
                Camera.main,
                out posLocal
            );

            panelRect.anchoredPosition = posLocal;
        }
    }
}