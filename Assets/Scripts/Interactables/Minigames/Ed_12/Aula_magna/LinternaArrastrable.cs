using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinternaArrastrable : MonoBehaviour, IDragHandler
{
    private bool puedeMoverse = false;
    private RectTransform rectTransform;
    private Canvas canvas;
    private MinijuegoLinterna minijuego;
    private Image imagen;
    private Vector2 tamañoOriginal;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        minijuego = GetComponentInParent<MinijuegoLinterna>();
        imagen = GetComponent<Image>();

        tamañoOriginal = rectTransform.sizeDelta;
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

        float limiteX = minijuego != null ? minijuego.ObtenerLimiteXLinterna() : 1000f;
        pos.x = Mathf.Clamp(pos.x, -limiteX, limiteX);

        rectTransform.anchoredPosition = new Vector2(pos.x, rectTransform.anchoredPosition.y);
    }

    public void ConfigurarLinterna(float radio, Color color)
    {
        if (imagen != null)
        {
            imagen.color = color;
        }

        float escalaNecesaria = (radio * 2) / tamañoOriginal.x;
        rectTransform.localScale = Vector3.one * escalaNecesaria;
    }
}
