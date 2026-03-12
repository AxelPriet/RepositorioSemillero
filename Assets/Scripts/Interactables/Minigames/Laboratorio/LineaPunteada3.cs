using UnityEngine;
using UnityEngine.UI;

public class LineaPunteada3 : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float anchoBarra = 40f;
    [SerializeField] private float altoBarra; 
    [SerializeField] private Color colorLinea = Color.white;
    [SerializeField] private float grosorLinea = 2f;
    [SerializeField] private float longitudSegmento = 10f;
    [SerializeField] private float espacioSegmento = 5f;

    private RectTransform rectTransform;
    private Image imagenLinea;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        CrearLineaPunteada();
    }

    private void CrearLineaPunteada()
    {
        GameObject lineaObj = new GameObject("LineaPunteada");
        lineaObj.transform.SetParent(transform, false);

        RectTransform lineaRect = lineaObj.AddComponent<RectTransform>();
        lineaRect.sizeDelta = new Vector2(anchoBarra, altoBarra);
        lineaRect.anchoredPosition = Vector2.zero;

        imagenLinea = lineaObj.AddComponent<Image>();
        imagenLinea.color = colorLinea;

        Texture2D textura = CrearTexturaPunteada();
        Sprite sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), Vector2.zero);
        imagenLinea.sprite = sprite;
        imagenLinea.material = new Material(Shader.Find("UI/Default"));
    }

    private Texture2D CrearTexturaPunteada()
    {
        int anchoTextura = 32;
        int altoTextura = 32;
        Texture2D textura = new Texture2D(anchoTextura, altoTextura);
        textura.filterMode = FilterMode.Point;

        Color[] pixeles = new Color[anchoTextura * altoTextura];

        for (int y = 0; y < altoTextura; y++)
        {
            for (int x = 0; x < anchoTextura; x++)
            {
                if ((x / (int)longitudSegmento + y / (int)longitudSegmento) % 2 == 0)
                    pixeles[y * anchoTextura + x] = Color.white;
                else
                    pixeles[y * anchoTextura + x] = Color.clear;
            }
        }

        textura.SetPixels(pixeles);
        textura.Apply();
        return textura;
    }

    public void SetAltura(float altura)
    {
        altoBarra = altura;
        if (rectTransform != null)
            rectTransform.sizeDelta = new Vector2(anchoBarra, altoBarra);
    }
}