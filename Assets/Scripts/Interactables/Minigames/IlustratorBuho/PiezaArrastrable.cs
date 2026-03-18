using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PiezaArrastrable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] public List<RectTransform> puntosValidos;
    [SerializeField] public float tolerancia = 30f;
    [SerializeField] public bool esOriginal;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool colocada = false;

    private GameObject clonEnArrastre;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (esOriginal)
        {
            clonEnArrastre = Instantiate(gameObject, canvas.transform);
            PiezaArrastrable clonScript = clonEnArrastre.GetComponent<PiezaArrastrable>();
            clonScript.esOriginal = false;
            clonScript.puntosValidos = puntosValidos;
            clonScript.tolerancia = tolerancia;

            CanvasGroup clonCG = clonEnArrastre.GetComponent<CanvasGroup>();
            if (clonCG == null) clonCG = clonEnArrastre.AddComponent<CanvasGroup>();
            clonCG.alpha = 0.7f;
            clonCG.blocksRaycasts = false;

            RectTransform clonRect = clonEnArrastre.GetComponent<RectTransform>();
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out pos
            );
            clonRect.anchoredPosition = pos;
        }
        else
        {
            if (colocada) return;
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (esOriginal)
        {
            if (clonEnArrastre == null) return;
            RectTransform clonRect = clonEnArrastre.GetComponent<RectTransform>();
            clonRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            if (colocada) return;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (esOriginal)
        {
            if (clonEnArrastre == null) return;

            RectTransform clonRect = clonEnArrastre.GetComponent<RectTransform>();
            RectTransform puntoCercano = ObtenerPuntoCercanoDeRect(clonRect);

            if (puntoCercano != null)
            {
                PuntoOcupado ocupado = puntoCercano.GetComponent<PuntoOcupado>();
                if (ocupado != null && ocupado.ocupado)
                {
                    Destroy(clonEnArrastre);
                    clonEnArrastre = null;
                    return;
                }

                PiezaArrastrable clonScript = clonEnArrastre.GetComponent<PiezaArrastrable>();
                clonScript.colocada = true;

                CanvasGroup clonCG = clonEnArrastre.GetComponent<CanvasGroup>();
                clonCG.alpha = 1f;
                clonCG.blocksRaycasts = false;

                clonRect.anchoredPosition = puntoCercano.anchoredPosition;

                if (ocupado == null)
                    ocupado = puntoCercano.gameObject.AddComponent<PuntoOcupado>();
                ocupado.ocupado = true;

                FindFirstObjectByType<MinijuegoBuho>()?.PiezaColocada();
            }
            else
            {
                Destroy(clonEnArrastre);
            }

            clonEnArrastre = null;
        }
        else
        {
            if (colocada) return;

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            RectTransform puntoCercano = ObtenerPuntoCercanoDeRect(rectTransform);

            if (puntoCercano != null)
            {
                PuntoOcupado ocupado = puntoCercano.GetComponent<PuntoOcupado>();
                if (ocupado != null && ocupado.ocupado)
                {
                    Destroy(gameObject);
                    return;
                }

                colocada = true;
                canvasGroup.blocksRaycasts = false;
                rectTransform.anchoredPosition = puntoCercano.anchoredPosition;

                if (ocupado == null)
                    ocupado = puntoCercano.gameObject.AddComponent<PuntoOcupado>();
                ocupado.ocupado = true;

                FindFirstObjectByType<MinijuegoBuho>()?.PiezaColocada();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private RectTransform ObtenerPuntoCercanoDeRect(RectTransform origen)
    {
        RectTransform puntoCercano = null;
        float distanciaMinima = tolerancia;

        foreach (RectTransform punto in puntosValidos)
        {
            float distancia = Vector2.Distance(origen.anchoredPosition, punto.anchoredPosition);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                puntoCercano = punto;
            }
        }
        return puntoCercano;
    }
}

public class PuntoOcupado : MonoBehaviour
{
    public bool ocupado = false;
}