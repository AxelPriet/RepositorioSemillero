using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PiezaArrastrable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private List<RectTransform> puntosValidos;
    [SerializeField] private float tolerancia = 30f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private GameObject clonActual;
    private bool piezaColocada = false;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (piezaColocada) return;

        // Crear clon
        clonActual = Instantiate(gameObject, canvas.transform);
        clonActual.transform.SetAsLastSibling();

        // Configurar clon
        PiezaArrastrable clonScript = clonActual.GetComponent<PiezaArrastrable>();
        clonScript.puntosValidos = puntosValidos;
        clonScript.enabled = true;

        // Posicionar clon donde está el mouse
        RectTransform clonRect = clonActual.GetComponent<RectTransform>();
        Vector2 posicionMouse;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out posicionMouse
        );
        clonRect.anchoredPosition = posicionMouse;

        // El clon es el que vamos a arrastrar
        rectTransform = clonRect;
        clonActual.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (piezaColocada || rectTransform == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (piezaColocada || rectTransform == null) return;

        rectTransform.GetComponent<Image>().color = Color.white;
        RectTransform puntoCercano = ObtenerPuntoCercano();

        if (puntoCercano != null)
        {
            rectTransform.anchoredPosition = puntoCercano.anchoredPosition;
            piezaColocada = true;
            FindFirstObjectByType<MinijuegoBuho>()?.PiezaColocada();
        }
        else
        {
            Destroy(rectTransform.gameObject);
        }
    }

    private RectTransform ObtenerPuntoCercano()
    {
        RectTransform puntoCercano = null;
        float distanciaMinima = tolerancia;

        foreach (RectTransform punto in puntosValidos)
        {
            float distancia = Vector2.Distance(rectTransform.anchoredPosition, punto.anchoredPosition);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                puntoCercano = punto;
            }
        }
        return puntoCercano;
    }
}