using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Implemento : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración")]
    [SerializeField] private string nombreImplemento;
    [SerializeField] private float distanciaSnap = 50f;
    [SerializeField] private int tamaño;
    private RectTransform canvasRect;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posicionInicial;
    private CanvasGroup canvasGroup;
    private bool colocado = false;
    private Camera camara;
    private Image imagen;
    private SlotEstante slotActual = null;
    private GameObject textoHover;
    private bool mouseSobreObjeto = false;

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
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (colocado) return;

        mouseSobreObjeto = true;
        CrearTextoHover();
    }

    private void CrearTextoHover()
    {
        textoHover = new GameObject("Hover_Tamaño");
        textoHover.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI texto = textoHover.AddComponent<TextMeshProUGUI>();
        texto.text = tamaño.ToString();
        texto.fontSize = 24;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;

        Outline outline = textoHover.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, 2);

        RectTransform textoRect = texto.GetComponent<RectTransform>();
        textoRect.sizeDelta = new Vector2(50, 50);

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector2 posicionDerecha = RectTransformUtility.WorldToScreenPoint(camara, corners[2]);

        Vector2 posLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            posicionDerecha + new Vector2(15, 0),
            canvas.worldCamera,
            out posLocal
        );

        textoRect.anchoredPosition = posLocal;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseSobreObjeto = false;
        if (textoHover != null)
        {
            Destroy(textoHover);
            textoHover = null;
        }
    }

    public int Tamaño
    {
        get { return tamaño; }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (textoHover != null)
        {
            Destroy(textoHover);
            textoHover = null;
        }

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        if (slotActual != null)
        {
            slotActual.RemoverImplemento(this);
            slotActual = null;
            colocado = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        SlotEstante slotCercano = BuscarSlotCercano();

        if (slotCercano != null && slotCercano.EspacioDisponible(tamaño))
        {
            rectTransform.position = slotCercano.transform.position;
            slotCercano.AgregarImplemento(this);
            slotActual = slotCercano;
            colocado = true;
            if (textoHover != null)
            {
                Destroy(textoHover);
                textoHover = null;
            }
        }
        else
        {
            StartCoroutine(FeedbackError());
            RegresarInicio();
        }
    }

    private SlotEstante BuscarSlotCercano()
    {
        SlotEstante[] todosLosSlots = FindObjectsByType<SlotEstante>(FindObjectsSortMode.None);
        SlotEstante masCercano = null;
        float distanciaMinima = distanciaSnap;

        foreach (var slot in todosLosSlots)
        {
            float distancia = Vector2.Distance(rectTransform.position, slot.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = slot;
            }
        }

        return masCercano;
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
        if (slotActual != null)
        {
            slotActual.RemoverImplemento(this);
            slotActual = null;
        }
        colocado = false;
        rectTransform.anchoredPosition = posicionInicial;
        canvasGroup.blocksRaycasts = true;
    }
}