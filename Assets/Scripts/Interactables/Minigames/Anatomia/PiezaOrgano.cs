using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PiezaOrgano : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Configuración")]
    [SerializeField] private string nombreOrgano;
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
    private MiniGame_Anatomia minijuego;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        camara = Camera.main;
        imagen = GetComponent<Image>();
        minijuego = FindFirstObjectByType<MiniGame_Anatomia>();

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
            ImagenZona imgZona = zona.GetComponent<ImagenZona>();
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
            PiezaOrgano organoOcupante = zona.GetComponentInChildren<PiezaOrgano>();
            if (organoOcupante != null && organoOcupante != this && organoOcupante.colocada)
                continue;

            float distancia = Vector2.Distance(rectTransform.position, zona.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                zonaDestacada = zona;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocada) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (zonaDestacada != null && zonaDestacada.name == posicionCorrecta)
        {
            rectTransform.position = zonaDestacada.position;
            colocada = true;
            canvasGroup.blocksRaycasts = false;
            transform.SetParent(zonaDestacada);

            ImagenZona imgZona = zonaDestacada.GetComponent<ImagenZona>();
            if (imgZona != null)
                imgZona.Completado();

            if (minijuego != null)
                minijuego.OrganoColocado();
        }
        else
        {
            RegresarInicio();

            if (zonaDestacada != null)
                StartCoroutine(FeedbackError());
        }

        foreach (var zona in todasLasPosiciones)
        {
            ImagenZona imgZona = zona.GetComponent<ImagenZona>();
            if (imgZona != null && zona != zonaDestacada)
                imgZona.ResetearColor();
        }
    }

    private void RegresarInicio()
    {
        rectTransform.anchoredPosition = posicionInicial;
        transform.SetParent(canvas.transform);
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
        transform.SetParent(canvas.transform);
    }
}