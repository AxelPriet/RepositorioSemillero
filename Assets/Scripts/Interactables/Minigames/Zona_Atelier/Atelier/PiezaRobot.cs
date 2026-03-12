using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PiezaRobot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private string nombrePieza;
    [SerializeField] private string posicionCorrecta;
    [SerializeField] private float distanciaSnap = 50f; 

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posicionInicial;
    private CanvasGroup canvasGroup;
    private bool colocada = false;
    private Camera camara;
    private Image imagen;

    private List<RectTransform> todasLasPosiciones = new List<RectTransform>();
    private RectTransform zonaDestacada = null;

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
        GameObject[] todosLosObjetos = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in todosLosObjetos)
        {
            if (obj.name.StartsWith("Posicion"))
            {
                RectTransform rect = obj.GetComponent<RectTransform>();
                if (rect != null)
                    todasLasPosiciones.Add(rect);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocada) return;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        foreach (var zona in todasLasPosiciones)
        {
            PosicionRobot imgZona = zona.GetComponent<PosicionRobot>();
            if (imgZona != null)
                imgZona.ResetearColor();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocada) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        DetectarZonaCercana();
    }

    private void DetectarZonaCercana()
    {
        zonaDestacada = null;
        float distanciaMinima = distanciaSnap;

        foreach (var zona in todasLasPosiciones)
        {
            PiezaRobot piezaOcupante = zona.GetComponentInChildren<PiezaRobot>();
            if (piezaOcupante != null && piezaOcupante != this && piezaOcupante.colocada)
                continue;

            float distancia = Vector2.Distance(rectTransform.position, zona.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                zonaDestacada = zona;
            }
        }

        if (zonaDestacada != null)
            Debug.Log($"Zona destacada para {nombrePieza}: '{zonaDestacada.name.Trim()}'");

        // Iluminar zonas
        foreach (var zona in todasLasPosiciones)
        {
            PosicionRobot imgZona = zona.GetComponent<PosicionRobot>();
            if (imgZona != null)
            {
                if (zona == zonaDestacada)
                    imgZona.Destacar();
                else
                    imgZona.ResetearColor();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocada) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Limpiar espacios y convertir a string simple
        string nombreZona = zonaDestacada != null ? zonaDestacada.name.Trim() : "";
        string nombreEsperado = posicionCorrecta.Trim();

        Debug.Log($"Comparando: '{nombreZona}' con '{nombreEsperado}'");

        if (zonaDestacada != null && nombreZona == nombreEsperado)
        {
            Debug.Log($"✅ {nombrePieza} colocada correctamente en {nombreZona}");

            // Colocar en la posición correcta
            rectTransform.position = zonaDestacada.position;
            colocada = true;
            canvasGroup.blocksRaycasts = false;

            // Marcar zona como ocupada
            PosicionRobot imgZona = zonaDestacada.GetComponent<PosicionRobot>();
            if (imgZona != null)
                imgZona.Completado();

            FindFirstObjectByType<MiniGame_Robot>()?.PiezaColocada(nombrePieza);
        }
        else
        {
            Debug.Log($"❌ {nombrePieza} - Esperaba: '{nombreEsperado}', Encontró: '{nombreZona}'");
            RegresarInicio();

            if (zonaDestacada != null)
                StartCoroutine(FeedbackError());
        }

        // Resetear colores
        foreach (var zona in todasLasPosiciones)
        {
            PosicionRobot imgZona = zona.GetComponent<PosicionRobot>();
            if (imgZona != null && zona != zonaDestacada)
                imgZona.ResetearColor();
        }
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
        colocada = false;
        rectTransform.anchoredPosition = posicionInicial;
        canvasGroup.blocksRaycasts = true;
    }
}