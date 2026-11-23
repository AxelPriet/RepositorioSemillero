using UnityEngine;
using UnityEngine.UI;

// Minimapa del HUD (pedido 2026-11-17), arriba a la derecha: una imagen simple del campus (rutas y edificios) con el
// jugador como un punto. "mundoMin/mundoMax" son las coordenadas del Grid que cubre la imagen. Con zoom 1 se ve el campus
// entero y el punto se mueve; con zoom > 1 el mapa se acerca y se desplaza para seguir al jugador.
public class Minimapa : MonoBehaviour
{
    [Tooltip("Ventana del mapa (con RectMask2D): lo que se ve")]
    public RectTransform ventana;
    [Tooltip("Imagen del campus, hija de la ventana")]
    public RectTransform mapa;
    [Tooltip("Punto del jugador, hijo de la ventana")]
    public RectTransform punto;
    [Tooltip("Texto opcional con el lugar actual, debajo del mapa")]
    public Text lugar;
    [Tooltip("Esquina inferior izquierda que cubre la imagen, en coordenadas del Grid")]
    public Vector2 mundoMin = new Vector2(-310, -150);
    [Tooltip("Esquina superior derecha que cubre la imagen, en coordenadas del Grid")]
    public Vector2 mundoMax = new Vector2(366, 121);
    [Tooltip("1 = campus entero; más = más cerca, siguiendo al jugador")]
    [Min(1f)] public float zoom = 1f;
    [Tooltip("Referencia del mundo (el Grid). Si está vacío se busca \"Grid\"")]
    public Transform grid;
    [Tooltip("Jugador. Si está vacío se busca el objeto con tag Player")]
    public Transform jugador;

    void LateUpdate()
    {
        if (ventana == null || mapa == null) return;
        if (jugador == null) { var j = GameObject.FindGameObjectWithTag("Player"); if (j != null) jugador = j.transform; }
        if (grid == null) { var g = GameObject.Find("Grid"); if (g != null) grid = g.transform; }

        Vector2 tam = ventana.rect.size;
        Vector2 mundo = mundoMax - mundoMin;
        float escala = tam.x / mundo.x * zoom;                       // px de UI por celda
        Vector2 tamMapa = mundo * escala;
        mapa.anchorMin = mapa.anchorMax = mapa.pivot = Vector2.zero;
        mapa.sizeDelta = tamMapa;

        Vector2 u = new Vector2(0.5f, 0.5f);
        if (jugador != null)
        {
            Vector3 p = grid != null ? grid.InverseTransformPoint(jugador.position) : jugador.position;
            u = new Vector2((p.x - mundoMin.x) / mundo.x, (p.y - mundoMin.y) / mundo.y);
        }
        Vector2 enMapa = Vector2.Scale(u, tamMapa);
        Vector2 desplaza = Vector2.zero;
        if (zoom > 1f || tamMapa.y > tam.y)                            // seguir al jugador sin salirse del mapa
        {
            desplaza = tam * 0.5f - enMapa;
            desplaza.x = Mathf.Clamp(desplaza.x, tam.x - tamMapa.x, 0f);
            desplaza.y = tamMapa.y > tam.y ? Mathf.Clamp(desplaza.y, tam.y - tamMapa.y, 0f) : (tam.y - tamMapa.y) * 0.5f;
        }
        else desplaza.y = (tam.y - tamMapa.y) * 0.5f;
        mapa.anchoredPosition = desplaza;

        if (punto != null)
        {
            punto.anchorMin = punto.anchorMax = Vector2.zero;
            punto.anchoredPosition = desplaza + enMapa;
            punto.gameObject.SetActive(jugador != null);
        }
        if (lugar != null) lugar.text = Lugar.Actual != null ? Cartelera.Pixel(Lugar.Actual.nombre) : "";
    }
}
