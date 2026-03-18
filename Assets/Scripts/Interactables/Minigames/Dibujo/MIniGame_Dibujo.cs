using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MiniGame_Dibujo : MonoBehaviour
{
    public enum TipoFigura { Circulo, Cuadrado, Triangulo }

    private class DatosFigura
    {
        public TipoFigura tipo;
        public List<Vector2> puntos = new List<Vector2>();
        public bool[] completados;
        public int totalCompletados = 0;
        public LineaUI lineaUI;
        public bool terminada = false;
        public float Progreso => totalCompletados / (float)puntos.Count;
    }

    [Header("Figuras")]
    [SerializeField] private float tamañoFigura = 150f;
    [SerializeField] private float grosorLinea = 4f;
    [SerializeField] private float toleranciaPunto = 18f;
    [SerializeField] private float toleranciaBorde = 20f;

    [Header("Posiciones (local en tablero)")]
    [SerializeField] private Vector2 posCirculo = new Vector2(-300f, 0f);
    [SerializeField] private Vector2 posCuadrado = new Vector2(0f, 0f);
    [SerializeField] private Vector2 posTriangulo = new Vector2(300f, 0f);

    [Header("Colores")]
    [SerializeField] private Color colorNoTrazado = Color.gray;
    [SerializeField] private Color colorTrazado = Color.green;

    [Header("Errores")]
    [SerializeField] private int erroresMaximos = 30;
    [SerializeField] private float cooldownError = 0.15f;

    [Header("UI")]
    [SerializeField] private RectTransform tablero;
    [SerializeField] private TextMeshProUGUI textoVidas;
    [SerializeField] private TextMeshProUGUI textoProgreso;
    [SerializeField] private TextMeshProUGUI textoMensaje;

    [Header("Escena")]
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";

    private List<DatosFigura> figuras = new List<DatosFigura>();
    private Camera camara;
    private bool dibujando = false;
    private int erroresActuales = 0;
    private bool juegoTerminado = false;
    private float tiempoUltimoError = -999f;
    private DatosFigura figuraActiva = null;

    private void Start()
    {
        camara = Camera.main;
        CrearFigura(TipoFigura.Circulo, posCirculo);
        CrearFigura(TipoFigura.Cuadrado, posCuadrado);
        CrearFigura(TipoFigura.Triangulo, posTriangulo);
        ActualizarUI();
    }

    // ─── Creación ───
    private void CrearFigura(TipoFigura tipo, Vector2 offset)
    {
        DatosFigura d = new DatosFigura();
        d.tipo = tipo;

        switch (tipo)
        {
            case TipoFigura.Circulo: GenerarCirculo(d, offset); break;
            case TipoFigura.Cuadrado: GenerarCuadrado(d, offset); break;
            case TipoFigura.Triangulo: GenerarTriangulo(d, offset); break;
        }

        d.completados = new bool[d.puntos.Count];

        GameObject go = new GameObject($"Figura_{tipo}");
        go.transform.SetParent(tablero, false);
        RectTransform rt = go.AddComponent<RectTransform>(); 
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

        LineaUI linea = go.AddComponent<LineaUI>();
        linea.puntos = d.puntos;
        linea.grosor = grosorLinea;
        linea.color = colorNoTrazado;
        linea.raycastTarget = false;
        linea.Refrescar();

        d.lineaUI = linea;
        figuras.Add(d);
    }

    private void GenerarCirculo(DatosFigura d, Vector2 offset)
    {
        int n = 64;
        float radio = tamañoFigura * 0.5f;
        for (int i = 0; i <= n; i++)
        {
            float a = (i / (float)n) * Mathf.PI * 2f;
            d.puntos.Add(offset + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radio);
        }
    }

    private void GenerarCuadrado(DatosFigura d, Vector2 offset)
    {
        float m = tamañoFigura * 0.5f;
        int pasos = 25;

        for (int i = 0; i <= pasos; i++) // inferior
            d.puntos.Add(offset + new Vector2(Mathf.Lerp(-m, m, i / (float)pasos), -m));
        for (int i = 1; i <= pasos; i++) // derecho
            d.puntos.Add(offset + new Vector2(m, Mathf.Lerp(-m, m, i / (float)pasos)));
        for (int i = 1; i <= pasos; i++) // superior
            d.puntos.Add(offset + new Vector2(Mathf.Lerp(m, -m, i / (float)pasos), m));
        for (int i = 1; i <= pasos; i++) // izquierdo
            d.puntos.Add(offset + new Vector2(-m, Mathf.Lerp(m, -m, i / (float)pasos)));
        d.puntos.Add(offset + new Vector2(-m, -m)); // cerrar
    }

    private void GenerarTriangulo(DatosFigura d, Vector2 offset)
    {
        // Triángulo equilátero centrado en offset
        float radio = tamañoFigura * 0.5f;
        int pasos = 30;

        // Los 3 vértices distribuidos en círculo, empezando arriba
        Vector2 v1 = offset + new Vector2(0, radio);                                         // arriba
        Vector2 v2 = offset + new Vector2(-radio * Mathf.Sqrt(3f) / 2f, -radio * 0.5f);       // abajo izquierda
        Vector2 v3 = offset + new Vector2(radio * Mathf.Sqrt(3f) / 2f, -radio * 0.5f);       // abajo derecha

        for (int i = 0; i <= pasos; i++) d.puntos.Add(Vector2.Lerp(v1, v2, i / (float)pasos));
        for (int i = 1; i <= pasos; i++) d.puntos.Add(Vector2.Lerp(v2, v3, i / (float)pasos));
        for (int i = 1; i <= pasos; i++) d.puntos.Add(Vector2.Lerp(v3, v1, i / (float)pasos));
        d.puntos.Add(v1); // cerrar al punto inicial
    }

    // ─── Update ───
    private void Update()
    {
        if (juegoTerminado || camara == null) return;

        Vector2 pos = ObtenerPosicionLocal();

        if (Mouse.current.leftButton.wasPressedThisFrame) dibujando = true;
        if (Mouse.current.leftButton.wasReleasedThisFrame) { dibujando = false; figuraActiva = null; }

        if (dibujando) ProcesarDibujo(pos);
    }

    private void ProcesarDibujo(Vector2 pos)
    {
        DatosFigura enBorde = null;
        float mejorDist = float.MaxValue;

        foreach (var fig in figuras)
        {
            if (fig.terminada) continue;
            float dist = DistanciaAlBorde(fig, pos);
            if (dist < toleranciaBorde && dist < mejorDist)
            {
                mejorDist = dist;
                enBorde = fig;
            }
        }

        if (enBorde != null)
        {
            figuraActiva = enBorde;
            MarcarPuntoCercano(enBorde, pos);
        }
        else if (figuraActiva != null && Time.time - tiempoUltimoError > cooldownError)
        {
            erroresActuales++;
            tiempoUltimoError = Time.time;
            ActualizarUI();

            if (erroresActuales >= erroresMaximos)
                StartCoroutine(PerderJuego());
        }
    }

    private void MarcarPuntoCercano(DatosFigura fig, Vector2 pos)
    {
        for (int i = 0; i < fig.puntos.Count; i++)
        {
            if (!fig.completados[i] && Vector2.Distance(pos, fig.puntos[i]) < toleranciaPunto)
            {
                fig.completados[i] = true;
                fig.totalCompletados++;
                fig.lineaUI.ActualizarColor(Color.Lerp(colorNoTrazado, colorTrazado, fig.Progreso));
            }
        }

        if (!fig.terminada && fig.totalCompletados >= fig.puntos.Count)
            StartCoroutine(FiguraCompletada(fig));
    }

    private float DistanciaAlBorde(DatosFigura fig, Vector2 pos)
    {
        float min = float.MaxValue;
        for (int i = 0; i < fig.puntos.Count - 1; i++)
        {
            float d = DistanciaPuntoSegmento(pos, fig.puntos[i], fig.puntos[i + 1]);
            if (d < min) min = d;
        }
        return min;
    }

    private float DistanciaPuntoSegmento(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float dot = Vector2.Dot(ab, ab);
        if (dot == 0f) return Vector2.Distance(p, a);
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / dot);
        return Vector2.Distance(p, a + t * ab);
    }

    // ─── Corrutinas ───
    private IEnumerator FiguraCompletada(DatosFigura fig)
    {
        fig.terminada = true;
        fig.lineaUI.ActualizarColor(colorTrazado);
        ActualizarUI();

        bool todas = true;
        foreach (var f in figuras) if (!f.terminada) { todas = false; break; }

        if (todas)
        {
            StartCoroutine(GanarJuego());
        }
        else
        {
            textoMensaje.text = $"¡{fig.tipo} completado!";
            yield return new WaitForSeconds(1f);
            textoMensaje.text = "";
        }
    }

    private IEnumerator GanarJuego()
    {
        juegoTerminado = true;
        textoMensaje.text = "¡COMPLETASTE TODO!";
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private IEnumerator PerderJuego()
    {
        juegoTerminado = true;
        dibujando = false;
        textoMensaje.text = "¡FALLASTE!\nDemasiados errores";
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoVidas.text = $"Errores: {erroresActuales}/{erroresMaximos}";
        float prog = 0f;
        foreach (var f in figuras) prog += f.Progreso;
        textoProgreso.text = $"Progreso: {prog / figuras.Count * 100f:F0}%";
    }

    private Vector2 ObtenerPosicionLocal()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            tablero,
            Mouse.current.position.ReadValue(),
            camara,
            out Vector2 pos
        );
        return pos;
    }
}