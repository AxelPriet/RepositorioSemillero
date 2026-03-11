using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PiezaInstrumento : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private string nombreInstrumento;
    [SerializeField] private float distanciaSnap = 50f; 

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posicionInicial;
    private CanvasGroup canvasGroup;
    private bool colocado = false;
    private Camera camara;

    private List<ZonaValor> todasLasZonas = new List<ZonaValor>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        camara = Camera.main;

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        posicionInicial = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        todasLasZonas.AddRange(FindObjectsByType<ZonaValor>(FindObjectsSortMode.None));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocado) return;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        foreach (var zona in todasLasZonas)
        {
            zona.IndicarSeleccion(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocado) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        DetectarZonaCercana();
    }

    private void DetectarZonaCercana()
    {
        Vector2 posicionMundo = rectTransform.position;
        ZonaValor zonaMasCercana = null;
        float distanciaMinima = distanciaSnap;

        foreach (var zona in todasLasZonas)
        {
            if (zona.EstaOcupada()) continue;

            float distancia = Vector2.Distance(posicionMundo, zona.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                zonaMasCercana = zona;
            }
        }

        foreach (var zona in todasLasZonas)
        {
            zona.IndicarSeleccion(zona == zonaMasCercana);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocado) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Vector2 posicionMundo = rectTransform.position;
        ZonaValor zonaMasCercana = null;
        float distanciaMinima = distanciaSnap;

        foreach (var zona in todasLasZonas)
        {
            if (zona.EstaOcupada()) continue;

            float distancia = Vector2.Distance(posicionMundo, zona.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                zonaMasCercana = zona;
            }
        }
        if (zonaMasCercana != null && zonaMasCercana.IntentarColocar(nombreInstrumento))
        {
            rectTransform.position = zonaMasCercana.transform.position;
            colocado = true;
            canvasGroup.blocksRaycasts = false;
            zonaMasCercana.IndicarCompletado();

            FindFirstObjectByType<MinijuegoLaboratorio>()?.InstrumentoColocado();
            Debug.Log($"¡{nombreInstrumento} colocado correctamente!");
        }
        else
        {
            StartCoroutine(FeedbackError());
            RegresarInicio();
        }

        foreach (var zona in todasLasZonas)
        {
            zona.IndicarSeleccion(false);
        }
    }

    private void RegresarInicio()
    {
        rectTransform.anchoredPosition = posicionInicial;
    }

    private IEnumerator FeedbackError()
    {
        Image img = GetComponent<Image>();
        Color original = img.color;
        img.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        img.color = original;
    }

    public void Resetear()
    {
        colocado = false;
        rectTransform.anchoredPosition = posicionInicial;
        canvasGroup.blocksRaycasts = true;
    }
}