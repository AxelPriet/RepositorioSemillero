using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinijuegoLinterna : MonoBehaviour
{
    [Header("Elementos del Juego")]
    [SerializeField] private RectTransform graduado;
    [SerializeField] private RectTransform linterna;
    [SerializeField] private float velocidadGraduado = 50f;

    [Header("Configuración Linterna")]
    [SerializeField] private float radioLinterna = 50f;
    [SerializeField] private float posicionYFija = 0f;

    [Header("UI Progreso")]
    [SerializeField] private Slider sliderProgreso;
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private float tiempoRequerido = 10f;
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";

    private float tiempoAcumulado = 0f;
    private bool minijuegoCompletado = false;

    private float limiteIzquierdo;
    private float limiteDerecho;
    private float limiteXLinterna;
    private Vector2 posicionInicialGraduado;

    private void Start()
    {
        CalcularLimites();
        ConfigurarUI();
        ConfigurarLinterna();
    }

    private void CalcularLimites()
    {
        RectTransform panelRect = graduado.parent as RectTransform;
        if (panelRect != null)
        {
            float anchoPanel = panelRect.rect.width;
            float mitadGraduado = graduado.rect.width / 2;

            limiteIzquierdo = -anchoPanel / 2 + mitadGraduado;
            limiteDerecho = anchoPanel / 2 - mitadGraduado;
            limiteXLinterna = anchoPanel / 2 - radioLinterna;
        }

        posicionInicialGraduado = new Vector2(limiteDerecho, 0);
        graduado.anchoredPosition = posicionInicialGraduado;
    }

    private void ConfigurarUI()
    {
        sliderProgreso.minValue = 0;
        sliderProgreso.maxValue = tiempoRequerido;
        sliderProgreso.value = 0;
        textoTiempo.text = $"{tiempoRequerido:F0}s";
        feedbackText.text = "Sigue al graduado con la linterna";
        textoInstrucciones.text = "Mueve el mouse horizontalmente";
    }

    private void ConfigurarLinterna()
    {
        Image imagenLinterna = linterna.GetComponent<Image>();
        if (imagenLinterna != null)
        {
            imagenLinterna.color = new Color(1, 1, 1, 0.2f);
        }
        linterna.sizeDelta = new Vector2(radioLinterna * 2, radioLinterna * 2);
        linterna.anchoredPosition = new Vector2(0, posicionYFija);
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        MoverGraduado();
        ActualizarPosicionLinterna();
        VerificarDeteccion();

        if (graduado.anchoredPosition.x <= limiteIzquierdo)
        {
            ReiniciarJuego();
        }
    }

    private void MoverGraduado()
    {
        Vector2 posicion = graduado.anchoredPosition;
        posicion.x -= velocidadGraduado * Time.deltaTime;
        graduado.anchoredPosition = posicion;
    }

    private void ActualizarPosicionLinterna()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                graduado.parent as RectTransform,
                mousePos,
                null,
                out Vector2 posicionMouse
            );
            float nuevaX = Mathf.Clamp(posicionMouse.x, -limiteXLinterna, limiteXLinterna);
            linterna.anchoredPosition = new Vector2(nuevaX, posicionYFija);
        }
    }

    private void VerificarDeteccion()
    {
        float distanciaX = Mathf.Abs(linterna.anchoredPosition.x - graduado.anchoredPosition.x);
        float distanciaY = Mathf.Abs(linterna.anchoredPosition.y - graduado.anchoredPosition.y);
        bool estaDentro = (distanciaX * distanciaX + distanciaY * distanciaY) <= (radioLinterna * radioLinterna);

        if (estaDentro)
        {
            tiempoAcumulado += Time.deltaTime;
            graduado.GetComponent<Image>().color = Color.white;
            sliderProgreso.value = tiempoAcumulado;
            textoTiempo.text = $"{tiempoRequerido - tiempoAcumulado:F1}s";

            if (tiempoAcumulado >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoAcumulado = 0f;
            sliderProgreso.value = 0;
            textoTiempo.text = $"{tiempoRequerido:F0}s";
            graduado.GetComponent<Image>().color = Color.gray;
        }
    }

    private void ReiniciarJuego()
    {
        graduado.anchoredPosition = posicionInicialGraduado;
        tiempoAcumulado = 0f;
        sliderProgreso.value = 0;
        textoTiempo.text = $"{tiempoRequerido:F0}s";
        feedbackText.text = "Llegó al final. ¡Inténtalo de nuevo!";
        StartCoroutine(FeedbackReinicio());
    }

    private IEnumerator FeedbackReinicio()
    {
        Color colorOriginal = graduado.GetComponent<Image>().color;
        graduado.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.3f);
        graduado.GetComponent<Image>().color = colorOriginal;
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡GRADUADO!";

        float tiempo = 0;
        while (tiempo < 0.5f)
        {
            graduado.localScale = Vector3.one * (1.2f + Mathf.Sin(tiempo * 30f) * 0.1f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}