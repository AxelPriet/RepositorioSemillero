using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PiezaArrastrable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private List<RectTransform> puntosValidos; // Arrastra aquí los puntos donde puede ir
    [SerializeField] private float tolerancia = 30f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private GameObject piezaActual;
    private bool colocada = false;
    private bool esOriginal;
    private Vector2 posicionInicial;
    private Image imagen;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        imagen = GetComponent<Image>();
        esOriginal = transform.parent.name == "BarraHerramientas";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocada) return;

        if (esOriginal)
        {
            // Clonar la pieza original
            piezaActual = Instantiate(gameObject, canvas.transform);
            PiezaArrastrable clon = piezaActual.GetComponent<PiezaArrastrable>();
            clon.esOriginal = false;
            clon.puntosValidos = puntosValidos; // Pasar los mismos puntos
            clon.transform.SetAsLastSibling();

            rectTransform = clon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            piezaActual = gameObject;
            rectTransform = GetComponent<RectTransform>();
            transform.SetAsLastSibling();
        }

        if (rectTransform != null)
        {
            imagen.color = new Color(1, 1, 1, 0.7f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocada || rectTransform == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocada || rectTransform == null) return;

        imagen.color = Color.white;

        // Buscar punto válido más cercano
        RectTransform puntoCercano = null;
        float distanciaMinima = tolerancia;

        foreach (RectTransform punto in puntosValidos)
        {
            // Verificar si el punto ya está ocupado
            if (punto.GetComponent<PuntoOcupado>() != null && punto.GetComponent<PuntoOcupado>().ocupado)
                continue;

            float distancia = Vector2.Distance(rectTransform.anchoredPosition, punto.anchoredPosition);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                puntoCercano = punto;
            }
        }

        if (puntoCercano != null)
        {
            // Colocar la pieza
            rectTransform.anchoredPosition = puntoCercano.anchoredPosition;
            colocada = true;

            // Marcar punto como ocupado
            PuntoOcupado po = puntoCercano.GetComponent<PuntoOcupado>();
            if (po == null)
                po = puntoCercano.gameObject.AddComponent<PuntoOcupado>();
            po.ocupado = true;

            FindFirstObjectByType<MinijuegoBuho>()?.PiezaColocada();
        }
        else if (!esOriginal)
        {
            // Si no es original y no se colocó, destruir el clon
            Destroy(gameObject);
        }
    }

    public void Resetear()
    {
        colocada = false;
        if (!esOriginal)
        {
            Destroy(gameObject);
        }
    }
}