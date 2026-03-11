using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinijuegoContorno : MonoBehaviour
{
    public enum TipoFigura { Circulo, Cuadrado, Triangulo }

    [Header("Configuración")]
    [SerializeField]
    private TipoFigura[] figurasOrden = {
        TipoFigura.Circulo,
        TipoFigura.Cuadrado,
        TipoFigura.Triangulo
    };
    [SerializeField] private float tamañoFigura = 200f;
    [SerializeField] private float grosorLinea = 3f;
    [SerializeField] private float tolerancia = 15f;
    [SerializeField] private Color colorNoTrazado = Color.gray;
    [SerializeField] private Color colorTrazado = Color.white;

    [Header("UI")]
    [SerializeField] private RectTransform tablero;
    [SerializeField] private TextMeshProUGUI textoFigura;
    [SerializeField] private TextMeshProUGUI textoProgreso;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";

    private LineRenderer lineaFigura;
    private List<Vector2> puntosFigura = new List<Vector2>();
    private bool[] puntosCompletados;
    private int puntosCompletadosTotal = 0;

    private int figuraActualIndex = 0;
    private TipoFigura figuraActual;
    private Camera camara;
    private bool dibujando = false;
    private Vector2 ultimaPosicion;
    private float distanciaEntrePuntos = 5f;

    private void Start()
    {
        camara = Camera.main;
        CrearLineRenderer();
        GenerarFigura(figurasOrden[0]);
        ActualizarUI();
    }

    private void CrearLineRenderer()
    {
        GameObject lineaObj = new GameObject("LineaContorno");
        lineaObj.transform.SetParent(tablero, false);

        lineaFigura = lineaObj.AddComponent<LineRenderer>();
        lineaFigura.startWidth = grosorLinea;
        lineaFigura.endWidth = grosorLinea;
        lineaFigura.material = new Material(Shader.Find("Sprites/Default"));
        lineaFigura.startColor = colorNoTrazado;
        lineaFigura.endColor = colorNoTrazado;
        lineaFigura.useWorldSpace = false;
    }

    private void GenerarFigura(TipoFigura tipo)
    {
        figuraActual = tipo;
        puntosFigura.Clear();

        switch (tipo)
        {
            case TipoFigura.Circulo:
                GenerarCirculo();
                textoFigura.text = "CÍRCULO";
                break;
            case TipoFigura.Cuadrado:
                GenerarCuadrado();
                textoFigura.text = "CUADRADO";
                break;
            case TipoFigura.Triangulo:
                GenerarTriangulo();
                textoFigura.text = "TRIÁNGULO";
                break;
        }

        Vector3[] puntos3D = new Vector3[puntosFigura.Count];
        for (int i = 0; i < puntosFigura.Count; i++)
            puntos3D[i] = new Vector3(puntosFigura[i].x, puntosFigura[i].y, 0);

        lineaFigura.positionCount = puntosFigura.Count;
        lineaFigura.SetPositions(puntos3D);
        lineaFigura.startColor = colorNoTrazado;
        lineaFigura.endColor = colorNoTrazado;

        puntosCompletados = new bool[puntosFigura.Count];
        puntosCompletadosTotal = 0;

        textoProgreso.text = "0%";
    }

    private void GenerarCirculo()
    {
        int numPuntos = 40;
        for (int i = 0; i <= numPuntos; i++)
        {
            float angulo = (i / (float)numPuntos) * Mathf.PI * 2f;
            float x = Mathf.Cos(angulo) * tamañoFigura / 2;
            float y = Mathf.Sin(angulo) * tamañoFigura / 2;
            puntosFigura.Add(new Vector2(x, y));
        }
    }

    private void GenerarCuadrado()
    {
        float mitad = tamañoFigura / 2f;
        puntosFigura.Add(new Vector2(-mitad, -mitad));
        puntosFigura.Add(new Vector2(mitad, -mitad));
        puntosFigura.Add(new Vector2(mitad, mitad));
        puntosFigura.Add(new Vector2(-mitad, mitad));
        puntosFigura.Add(new Vector2(-mitad, -mitad));
    }

    private void GenerarTriangulo()
    {
        float altura = tamañoFigura * Mathf.Sqrt(3) / 2f;
        puntosFigura.Add(new Vector2(0, altura / 2));
        puntosFigura.Add(new Vector2(-tamañoFigura / 2, -altura / 2));
        puntosFigura.Add(new Vector2(tamañoFigura / 2, -altura / 2));
        puntosFigura.Add(new Vector2(0, altura / 2));
    }

    private void Update()
    {
        if (camara == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            EmpezarADibujar();
        }

        if (Mouse.current.leftButton.IsPressed() && dibujando)
        {
            Dibujar();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TerminarDeDibujar();
        }
    }

    private Vector2 ObtenerPosicionLocal()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            tablero,
            Mouse.current.position.ReadValue(),
            camara,
            out Vector2 posLocal
        );
        return posLocal;
    }

    private void EmpezarADibujar()
    {
        Vector2 pos = ObtenerPosicionLocal();

        int puntoCercano = PuntoMasCercano(pos);
        if (puntoCercano < 0) return;

        dibujando = true;
        ultimaPosicion = pos;
    }

    private void Dibujar()
    {
        Vector2 posActual = ObtenerPosicionLocal();

        if (Vector2.Distance(posActual, ultimaPosicion) >= distanciaEntrePuntos)
        {
            int puntoCercano = PuntoMasCercano(posActual);

            if (puntoCercano >= 0 && !puntosCompletados[puntoCercano])
            {
                puntosCompletados[puntoCercano] = true;
                puntosCompletadosTotal++;

                ActualizarSegmentosLinea();

                ActualizarProgreso();
            }

            ultimaPosicion = posActual;
        }
    }

    private int PuntoMasCercano(Vector2 punto)
    {
        int puntoCercano = -1;
        float distanciaMinima = tolerancia;

        for (int i = 0; i < puntosFigura.Count; i++)
        {
            float distancia = Vector2.Distance(punto, puntosFigura[i]);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                puntoCercano = i;
            }
        }
        return puntoCercano;
    }

    private void ActualizarSegmentosLinea()
    {
        float progreso = puntosCompletadosTotal / (float)puntosFigura.Count;
        Color nuevoColor = Color.Lerp(colorNoTrazado, colorTrazado, progreso);

        lineaFigura.startColor = nuevoColor;
        lineaFigura.endColor = nuevoColor;
    }

    private void ActualizarProgreso()
    {
        float progreso = (puntosCompletadosTotal / (float)puntosFigura.Count) * 100f;
        textoProgreso.text = $"{progreso:F1}%";

        if (puntosCompletadosTotal >= puntosFigura.Count)
        {
            StartCoroutine(CompletarFigura());
        }
    }

    private void TerminarDeDibujar()
    {
        dibujando = false;
    }

    private IEnumerator CompletarFigura()
    {
        dibujando = false;
        textoInstrucciones.text = "¡FIGURA COMPLETADA!";
        yield return new WaitForSeconds(1f);

        figuraActualIndex++;
        if (figuraActualIndex < figurasOrden.Length)
        {
            GenerarFigura(figurasOrden[figuraActualIndex]);
            textoInstrucciones.text = "Siguiente figura...";
            yield return new WaitForSeconds(1f);
            textoInstrucciones.text = "Traza el contorno";
        }
        else
        {
            StartCoroutine(GanarJuego());
        }
    }

    private IEnumerator GanarJuego()
    {
        textoInstrucciones.text = "¡COMPLETASTE TODO!";
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoProgreso.text = "0%";
        textoInstrucciones.text = "Traza el contorno";
    }
}