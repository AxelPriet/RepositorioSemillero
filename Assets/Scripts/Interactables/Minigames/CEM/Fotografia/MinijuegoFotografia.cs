using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinijuegoFotografia : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform marco;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoTemporizador;
    [SerializeField] private Image barraProgreso;
    [SerializeField] private Image flashImage; 

    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;
    [SerializeField] private float tiempoRequerido = 2f;

    [Header("Referencias")]
    [SerializeField] private Canvas canvas;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";

    private PlayerControls playerControls;
    private MarcoArrastrable marcoScript;
    private Vector3 marcoPosInicial;
    private float tiempoAcumulado = 0f;
    private bool puedeTomarFoto = true;
    private bool fotoTomada = false;

    private void Awake()
    {
        marcoScript = marco.GetComponent<MarcoArrastrable>();
        if (marcoScript == null)
            marcoScript = marco.gameObject.AddComponent<MarcoArrastrable>();
    }

    private void Start()
    {
        marcoPosInicial = marco.position;
        Debug.Log($"Marco posición inicial: {marcoPosInicial}");

        playerControls = InputHandler.Instance.GetControls();
        playerControls.Gameplay.Compress.performed += OnTomarFoto;

        marcoScript.SetPuedeMoverse(true);
        Debug.Log("Marco puede moverse: true");

        if (flashImage != null)
            flashImage.gameObject.SetActive(false);

        ActualizarUI();
    }

    private void Update()
    {
        if (!puedeTomarFoto || fotoTomada) return;

        bool dentro = EstaObjetivoEnMarco();

        Color colorMarco = dentro ? Color.green : Color.white;
        marco.GetComponent<Image>().color = colorMarco;

        if (dentro)
        {
            tiempoAcumulado += Time.deltaTime;

            float progreso = tiempoAcumulado / tiempoRequerido;
            if (barraProgreso != null)
                barraProgreso.fillAmount = progreso;

            float tiempoRestante = tiempoRequerido - tiempoAcumulado;
            textoTemporizador.text = $"{tiempoRestante:F1}s";

            if (tiempoAcumulado >= tiempoRequerido && !fotoTomada)
            {
                StartCoroutine(TomarFotoExitosa());
            }
        }
        else
        {
            tiempoAcumulado = Mathf.Max(0, tiempoAcumulado - Time.deltaTime * 2f);

            if (barraProgreso != null)
                barraProgreso.fillAmount = tiempoAcumulado / tiempoRequerido;

            textoTemporizador.text = $"{tiempoRequerido:F0}s";
        }
    }

    private bool EstaObjetivoEnMarco()
    {
        if (objetivo == null || marco == null) return false;

        Vector3 objetivoScreenPos = Camera.main.WorldToScreenPoint(objetivo.position);

        Camera camaraCanvas = canvas.worldCamera;
        if (camaraCanvas == null) camaraCanvas = Camera.main;

        RectTransform marcoRect = marco;
        Vector3[] esquinas = new Vector3[4];
        marcoRect.GetWorldCorners(esquinas);

        Vector2[] esquinasPantalla = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            esquinasPantalla[i] = camaraCanvas.WorldToScreenPoint(esquinas[i]);
        }

        float minX = Mathf.Min(esquinasPantalla[0].x, esquinasPantalla[1].x, esquinasPantalla[2].x, esquinasPantalla[3].x);
        float minY = Mathf.Min(esquinasPantalla[0].y, esquinasPantalla[1].y, esquinasPantalla[2].y, esquinasPantalla[3].y);
        float maxX = Mathf.Max(esquinasPantalla[0].x, esquinasPantalla[1].x, esquinasPantalla[2].x, esquinasPantalla[3].x);
        float maxY = Mathf.Max(esquinasPantalla[0].y, esquinasPantalla[1].y, esquinasPantalla[2].y, esquinasPantalla[3].y);

        Rect rectMarco = new Rect(minX, minY, maxX - minX, maxY - minY);

        return rectMarco.Contains(new Vector2(objetivoScreenPos.x, objetivoScreenPos.y));
    }

    private IEnumerator TomarFotoExitosa()
    {
        fotoTomada = true;
        marcoScript.SetPuedeMoverse(false);

        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            flashImage.gameObject.SetActive(false);
        }

        textoInstrucciones.text = "¡Foto perfecta!";
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);

    }

    private void OnTomarFoto(InputAction.CallbackContext context)
    {
        if (!puedeTomarFoto || fotoTomada) return;

        if (EstaObjetivoEnMarco())
        {
            StartCoroutine(TomarFotoExitosa());
        }
        else
        {
            StartCoroutine(FeedbackError());
        }
    }

    private IEnumerator FeedbackError()
    {
        marco.GetComponent<Image>().color = Color.red;
        textoInstrucciones.text = "¡No está encuadrado!";
        yield return new WaitForSeconds(0.5f);
        marco.GetComponent<Image>().color = Color.white;
        textoInstrucciones.text = "Encuadra el objetivo " + tiempoRequerido + " segundos";
    }

    private void ActualizarUI()
    {
        textoInstrucciones.text = "Encuadra el objetivo " + tiempoRequerido + " segundos";
        textoTemporizador.text = $"{tiempoRequerido:F0}s";
        if (barraProgreso != null)
            barraProgreso.fillAmount = 0;
    }

    private void OnDestroy()
    {
        if (playerControls != null)
            playerControls.Gameplay.Compress.performed -= OnTomarFoto;
    }
}