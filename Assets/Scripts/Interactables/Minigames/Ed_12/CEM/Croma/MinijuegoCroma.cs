using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class MinijuegoCroma : MonoBehaviour
{
    [Header("Sliders RGB")]
    [SerializeField] private Slider sliderRojo;
    [SerializeField] private Slider sliderVerde;
    [SerializeField] private Slider sliderAzul;

    [Header("Textos de valores (sliders)")]
    [SerializeField] private TextMeshProUGUI textoValorRojo;
    [SerializeField] private TextMeshProUGUI textoValorVerde;
    [SerializeField] private TextMeshProUGUI textoValorAzul;

    [Header("Textos objetivo")]
    [SerializeField] private TextMeshProUGUI textoObjetivoRojo;
    [SerializeField] private TextMeshProUGUI textoObjetivoVerde;
    [SerializeField] private TextMeshProUGUI textoObjetivoAzul;

    [Header("UI Visual")]
    [SerializeField] private Image luzRoja;
    [SerializeField] private Image luzVerde;
    [SerializeField] private Image luzAzul;
    [SerializeField] private Image colorObjetivo;
    [SerializeField] private Image colorActual;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private float tiempoRequerido = 2f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";
    [SerializeField] private int minigameIndex ;

    [Header("Control por teclado")]
    [SerializeField] private TextMeshProUGUI textoCanalActivo;
    [SerializeField] private float pasoTeclado = 1f / 255f;

    private float valorObjetivoRojo;
    private float valorObjetivoVerde;
    private float valorObjetivoAzul;
    private float tiempoCorrecto = 0f;
    private bool minijuegoCompletado = false;
    private int selectedChannel = 0;
    private Color colorHandleRojo, colorHandleVerde, colorHandleAzul;
    private PlayerControls playerControls;

    private void Start()
    {
        // Obtener PlayerControls
        playerControls = InputHandler.Instance?.GetControls();
        if (playerControls != null)
        {
            playerControls.Gameplay.SelectRed.performed += _ => SelectChannel(0);
            playerControls.Gameplay.SelectGreen.performed += _ => SelectChannel(1);
            playerControls.Gameplay.SelectBlue.performed += _ => SelectChannel(2);
            playerControls.Gameplay.AdjustUp.started += _ => AjustarValorActualHaciaArriba();
            playerControls.Gameplay.AdjustDown.started += _ => AjustarValorActualHaciaAbajo();
            playerControls.Gameplay.Enable();
        }

        GenerarColorObjetivo();
        ResetearSliders();
        ActualizarUI();

        if (sliderRojo.handleRect != null)
            colorHandleRojo = sliderRojo.handleRect.GetComponent<Image>().color;
        if (sliderVerde.handleRect != null)
            colorHandleVerde = sliderVerde.handleRect.GetComponent<Image>().color;
        if (sliderAzul.handleRect != null)
            colorHandleAzul = sliderAzul.handleRect.GetComponent<Image>().color;

        sliderRojo.onValueChanged.AddListener(v => ActualizarTextoSlider(v, textoValorRojo));
        sliderVerde.onValueChanged.AddListener(v => ActualizarTextoSlider(v, textoValorVerde));
        sliderAzul.onValueChanged.AddListener(v => ActualizarTextoSlider(v, textoValorAzul));

        SelectChannel(0);
    }

    private void OnDestroy()
    {
        sliderRojo.onValueChanged.RemoveAllListeners();
        sliderVerde.onValueChanged.RemoveAllListeners();
        sliderAzul.onValueChanged.RemoveAllListeners();

        if (playerControls != null)
        {
            playerControls.Gameplay.SelectRed.performed -= _ => SelectChannel(0);
            playerControls.Gameplay.SelectGreen.performed -= _ => SelectChannel(1);
            playerControls.Gameplay.SelectBlue.performed -= _ => SelectChannel(2);
            playerControls.Gameplay.AdjustUp.started -= _ => AjustarValorActualHaciaArriba();
            playerControls.Gameplay.AdjustDown.started -= _ => AjustarValorActualHaciaAbajo();
        }
    }

    private void Update()
    {
        if (minijuegoCompletado) return;
        ActualizarLuces();
        ActualizarColorMezclado();
        VerificarMezcla();
    }

    private void AjustarValorActualHaciaArriba()
    {
        if (minijuegoCompletado) return;
        Slider sliderActual = selectedChannel == 0 ? sliderRojo :
                             (selectedChannel == 1 ? sliderVerde : sliderAzul);
        float valorActual = Mathf.Round(sliderActual.value * 255f);
        sliderActual.value = Mathf.Clamp01((valorActual + 1f) / 255f);
    }

    private void AjustarValorActualHaciaAbajo()
    {
        if (minijuegoCompletado) return;
        Slider sliderActual = selectedChannel == 0 ? sliderRojo :
                             (selectedChannel == 1 ? sliderVerde : sliderAzul);
        float valorActual = Mathf.Round(sliderActual.value * 255f);
        sliderActual.value = Mathf.Clamp01((valorActual - 1f) / 255f);
    }

    private void SelectChannel(int channel)
    {
        selectedChannel = channel;
        string nombre = channel == 0 ? "ROJO" : (channel == 1 ? "VERDE" : "AZUL");
        if (textoCanalActivo) textoCanalActivo.text = $"Ajustando: {nombre}";

        ResetearResaltados();
        ResaltarSlider(channel);
    }

    private void ResetearResaltados()
    {
        if (sliderRojo.handleRect != null)
            sliderRojo.handleRect.GetComponent<Image>().color = colorHandleRojo;
        if (sliderVerde.handleRect != null)
            sliderVerde.handleRect.GetComponent<Image>().color = colorHandleVerde;
        if (sliderAzul.handleRect != null)
            sliderAzul.handleRect.GetComponent<Image>().color = colorHandleAzul;
    }

    private void ResaltarSlider(int channel)
    {
        Color highlight = Color.yellow;
        if (channel == 0 && sliderRojo.handleRect != null)
            sliderRojo.handleRect.GetComponent<Image>().color = highlight;
        else if (channel == 1 && sliderVerde.handleRect != null)
            sliderVerde.handleRect.GetComponent<Image>().color = highlight;
        else if (channel == 2 && sliderAzul.handleRect != null)
            sliderAzul.handleRect.GetComponent<Image>().color = highlight;
    }

    private void GenerarColorObjetivo()
    {
        valorObjetivoRojo = Mathf.RoundToInt(Random.Range(0.2f, 0.8f) * 255f) / 255f;
        valorObjetivoVerde = Mathf.RoundToInt(Random.Range(0.2f, 0.8f) * 255f) / 255f;
        valorObjetivoAzul = Mathf.RoundToInt(Random.Range(0.2f, 0.8f) * 255f) / 255f;
        colorObjetivo.color = new Color(valorObjetivoRojo, valorObjetivoVerde, valorObjetivoAzul);
        ActualizarTextosObjetivo();
    }

    private void ActualizarTextosObjetivo()
    {
        if (textoObjetivoRojo) textoObjetivoRojo.text = $"{Mathf.RoundToInt(valorObjetivoRojo * 255)}";
        if (textoObjetivoVerde) textoObjetivoVerde.text = $"{Mathf.RoundToInt(valorObjetivoVerde * 255)}";
        if (textoObjetivoAzul) textoObjetivoAzul.text = $"{Mathf.RoundToInt(valorObjetivoAzul * 255)}";
    }

    private void ResetearSliders()
    {
        sliderRojo.value = 0f;
        sliderVerde.value = 0f;
        sliderAzul.value = 0f;
        ActualizarTextoSlider(0f, textoValorRojo);
        ActualizarTextoSlider(0f, textoValorVerde);
        ActualizarTextoSlider(0f, textoValorAzul);
    }

    private void ActualizarTextoSlider(float valor, TextMeshProUGUI texto)
    {
        if (texto) texto.text = $"{Mathf.RoundToInt(valor * 255)}";
    }

    private void ActualizarUI()
    {
        feedbackText.text = "Iguala el color objetivo usando los sliders";
        textoInstrucciones.text = "1=R | 2=G | 3=B | ↑ ↓ para ajustar";
    }

    private void ActualizarLuces()
    {
        if (luzRoja) luzRoja.color = Color.red * sliderRojo.value;
        if (luzVerde) luzVerde.color = Color.green * sliderVerde.value;
        if (luzAzul) luzAzul.color = Color.blue * sliderAzul.value;
    }

    private void ActualizarColorMezclado()
    {
        Color mezcla = new Color(sliderRojo.value, sliderVerde.value, sliderAzul.value);
        if (colorActual) colorActual.color = mezcla;
    }

    private void VerificarMezcla()
    {
        int rojoActual = Mathf.RoundToInt(sliderRojo.value * 255f);
        int verdeActual = Mathf.RoundToInt(sliderVerde.value * 255f);
        int azulActual = Mathf.RoundToInt(sliderAzul.value * 255f);

        int rojoObj = Mathf.RoundToInt(valorObjetivoRojo * 255f);
        int verdeObj = Mathf.RoundToInt(valorObjetivoVerde * 255f);
        int azulObj = Mathf.RoundToInt(valorObjetivoAzul * 255f);

        bool rojoCorrecto = rojoActual == rojoObj;
        bool verdeCorrecto = verdeActual == verdeObj;
        bool azulCorrecto = azulActual == azulObj;
        bool todoCorrecto = rojoCorrecto && verdeCorrecto && azulCorrecto;

        if (todoCorrecto)
        {
            tiempoCorrecto += Time.deltaTime;
            float tiempoRestante = tiempoRequerido - tiempoCorrecto;
            feedbackText.text = $"¡Color exacto! Mantén {tiempoRestante:F1}s";
            float escala = 1f + (tiempoCorrecto / tiempoRequerido) * 0.2f;
            if (colorActual) colorActual.transform.localScale = Vector3.one * escala;
            if (tiempoCorrecto >= tiempoRequerido)
                StartCoroutine(CompletarMinijuego());
        }
        else
        {
            tiempoCorrecto = 0f;
            if (colorActual) colorActual.transform.localScale = Vector3.one;
            string mensaje = "";
            if (!rojoCorrecto) mensaje += $"Rojo (debe ser {rojoObj}), ";
            if (!verdeCorrecto) mensaje += $"Verde (debe ser {verdeObj}), ";
            if (!azulCorrecto) mensaje += $"Azul (debe ser {azulObj})";
            feedbackText.text = string.IsNullOrEmpty(mensaje)
                ? "Ajusta los sliders al valor mostrado"
                : $"Ajusta: {mensaje.TrimEnd(',', ' ')}";
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡CROMA PERFECTO!";
        float tiempo = 0;
        while (tiempo < 0.5f)
        {
            float escala = 1.2f + Mathf.Sin(tiempo * 20f) * 0.1f;
            if (colorActual) colorActual.transform.localScale = Vector3.one * escala;
            tiempo += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinCroma);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}