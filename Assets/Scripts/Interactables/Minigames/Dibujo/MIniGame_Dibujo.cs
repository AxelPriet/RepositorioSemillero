using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinijuegoDibujo : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float distanciaEntrePuntos = 10f;
    [SerializeField] private float progresoRequerido = 80f;

    [Header("Elementos")]
    [SerializeField] private RectTransform lienzo;
    [SerializeField] private RectTransform zonaTrazado;
    [SerializeField] private GameObject puntoPrefab;
    [SerializeField] private DetectorFigura detector;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoFigura;
    [SerializeField] private TextMeshProUGUI textoProgreso;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";

    private Camera camara;
    private bool dibujando = false;
    private Vector2 ultimaPosicion;
    private List<GameObject> puntosCreados = new List<GameObject>();
    private float progreso = 0f;
    private bool figuraCompletada = false;

    private void Start()
    {
        camara = Camera.main;
        textoInstrucciones.text = "Mantén clic y traza la figura";
    }

    private void Update()
    {
        if (figuraCompletada) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && MouseEnZonaValida())
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

    private bool MouseEnZonaValida()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            zonaTrazado,
            Mouse.current.position.ReadValue(),
            camara
        );
    }

    private Vector2 ObtenerPosicionLocal()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lienzo,
            Mouse.current.position.ReadValue(),
            camara,
            out Vector2 posLocal
        );
        return posLocal;
    }

    private void EmpezarADibujar()
    {
        dibujando = true;
        ultimaPosicion = ObtenerPosicionLocal();
    }

    private void Dibujar()
    {
        Vector2 posActual = ObtenerPosicionLocal();

        if (Vector2.Distance(posActual, ultimaPosicion) >= distanciaEntrePuntos)
        {
            if (detector.VerificarPunto(posActual))
            {
                CrearPunto(posActual, Color.green);
            }
            else
            {
                CrearPunto(posActual, Color.red);
            }

            ultimaPosicion = posActual;
            ActualizarProgreso();
        }
    }

    private void CrearPunto(Vector2 posicion, Color color)
    {
        GameObject punto = Instantiate(puntoPrefab, lienzo);
        punto.GetComponent<RectTransform>().anchoredPosition = posicion;
        punto.GetComponent<Image>().color = color;
        puntosCreados.Add(punto);
    }

    private void ActualizarProgreso()
    {
        progreso = detector.ObtenerProgreso();
        textoProgreso.text = $"{progreso:F0}%";

        if (progreso >= progresoRequerido)
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
        figuraCompletada = true;
        textoInstrucciones.text = "¡FIGURA COMPLETADA!";
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}