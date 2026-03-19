using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinijuegoFotografia : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform marco;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private Image flashImage;

    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;

    [Header("Referencias")]
    [SerializeField] private Canvas canvas;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PlayerControls playerControls;
    private MarcoArrastrable marcoScript;
    private Vector3 marcoPosInicial;
    private bool puedeTomarFoto = true;
    private bool fotoTomada = false;
    [SerializeField] private int minigameIndex;


    private void Awake()
    {
        marcoScript = marco.GetComponent<MarcoArrastrable>();
        if (marcoScript == null)
            marcoScript = marco.gameObject.AddComponent<MarcoArrastrable>();
    }

    private void Start()
    {
        Vector3 pos = marco.position;
        pos.z = 0;
        marco.position = pos;

        marcoPosInicial = marco.position;

        playerControls = InputHandler.Instance.GetControls();
        playerControls.Gameplay.Compress.performed += OnTomarFoto;

        marcoScript.SetPuedeMoverse(true);

        if (flashImage != null)
            flashImage.gameObject.SetActive(false);

        ActualizarUI();
    }

    private void Update()
    {
        if (!puedeTomarFoto || fotoTomada) return;

        bool dentro = EstaObjetivoEnMarco();
        marco.GetComponent<Image>().color = dentro ? Color.green : Color.white;
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
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
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
        textoInstrucciones.text = "Encuadra el objetivo y presiona ESPACIO";
    }

    private void ActualizarUI()
    {
        textoInstrucciones.text = "Encuadra el objetivo y presiona ESPACIO";
    }

    private void OnDestroy()
    {
        if (playerControls != null)
            playerControls.Gameplay.Compress.performed -= OnTomarFoto;
    }
}