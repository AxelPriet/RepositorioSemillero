using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinijuegoLinterna : MonoBehaviour
{
    [Header("Elementos del Juego")]
    [SerializeField] private RectTransform graduado;
    [SerializeField] private RectTransform linterna;
    [SerializeField] private float velocidadGraduado = 50f;

    [Header("Referencias")]
    [SerializeField] private LinternaArrastrable linternaScript;

    [Header("Configuración Linterna")]
    [SerializeField] private float radioLinterna = 50f;

    [Header("UI Progreso")]
    [SerializeField] private TextMeshProUGUI textoTiempo;

    [Header("Configuración")]
    [SerializeField] private float tiempoRequerido = 10f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";
    [SerializeField] private int minigameIndex;

    private float tiempoAcumulado = 0f;
    private bool minijuegoCompletado = false;
    private float limiteIzquierdo;
    private float limiteXLinterna;
    private Vector2 posicionInicialGraduado;

    private void Start()
    {
        CalcularLimites();
        ConfigurarLinterna();

        if (linternaScript != null)
            linternaScript.SetPuedeMoverse(true);
    }

    private void CalcularLimites()
    {
        RectTransform panelRect = graduado.parent as RectTransform;
        if (panelRect != null)
        {
            float anchoPanel = panelRect.rect.width;
            float mitadGraduado = graduado.rect.width / 2;

            limiteIzquierdo = -anchoPanel / 2 + mitadGraduado;
            limiteXLinterna = anchoPanel / 2 - radioLinterna;
        }

        posicionInicialGraduado = graduado.anchoredPosition;
    }

    private void ConfigurarLinterna()
    {
        if (linternaScript != null)
        {
            Color colorLinterna = new Color(1, 1, 1, 0.2f);
            linternaScript.ConfigurarLinterna(radioLinterna, colorLinterna);
        }
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        MoverGraduado();
        VerificarDeteccion();

        if (graduado.anchoredPosition.x <= limiteIzquierdo)
            ReiniciarJuego();
    }

    private void MoverGraduado()
    {
        Vector2 posicion = graduado.anchoredPosition;
        posicion.x -= velocidadGraduado * Time.deltaTime;
        graduado.anchoredPosition = posicion;
    }

    public float ObtenerLimiteXLinterna()
    {
        return limiteXLinterna;
    }

    private void VerificarDeteccion()
    {
        // Solo comparar en X ya que el movimiento es horizontal
        float distanciaX = Mathf.Abs(linterna.anchoredPosition.x - graduado.anchoredPosition.x);
        bool estaDentro = distanciaX <= radioLinterna;

        if (estaDentro)
        {
            tiempoAcumulado += Time.deltaTime;
            graduado.GetComponent<Image>().color = Color.white;
            textoTiempo.text = $"{tiempoRequerido - tiempoAcumulado:F1}s";

            if (tiempoAcumulado >= tiempoRequerido)
                StartCoroutine(CompletarMinijuego());
        }
        else
        {
            tiempoAcumulado = 0f;
            textoTiempo.text = $"{tiempoRequerido:F0}s";
            graduado.GetComponent<Image>().color = Color.gray;
        }
    }

    private void ReiniciarJuego()
    {
        graduado.anchoredPosition = posicionInicialGraduado;
        tiempoAcumulado = 0f;
        textoTiempo.text = $"{tiempoRequerido:F0}s";
        StartCoroutine(FeedbackReinicio());
    }

    private IEnumerator FeedbackReinicio()
    {
        Image imgGraduado = graduado.GetComponent<Image>();
        imgGraduado.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        imgGraduado.color = Color.gray;
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;

        float tiempo = 0;
        while (tiempo < 0.5f)
        {
            graduado.localScale = Vector3.one * (1.2f + Mathf.Sin(tiempo * 30f) * 0.1f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinAulaMagna);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}