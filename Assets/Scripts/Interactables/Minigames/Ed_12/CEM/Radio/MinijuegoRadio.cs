using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinijuegoRadio : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider[] sliders;              

    [Header("Textos de valores actuales")]
    [SerializeField] private TextMeshProUGUI[] textosValorActual;

    [Header("Textos de valores objetivo")]
    [SerializeField] private TextMeshProUGUI[] textosValorObjetivo;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private Image cartelOnAir;

    [Header("Configuración")]
    [SerializeField] private float tolerancia = 0.01f; 
    [SerializeField] private float tiempoRequerido = 3f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";
    [SerializeField] private int minigameIndex;

    private float[] valoresObjetivo;
    private float tiempoCorrecto = 0f;
    private bool minijuegoCompletado = false;

    private void Start()
    {
        if (sliders.Length != textosValorActual.Length || sliders.Length != textosValorObjetivo.Length)
        {
            Debug.LogError("La cantidad de sliders, textos de valor actual y textos objetivo no coincide.");
            return;
        }

        GenerarValoresAleatorios();
        ConfigurarSliders();
        ResetearUI();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ActualizarTextosValoresActuales();
        VerificarSeñal();
    }

    private void GenerarValoresAleatorios()
    {
        valoresObjetivo = new float[sliders.Length];
        for (int i = 0; i < sliders.Length; i++)
        {
            int objetivoInt = Random.Range(20, 81);
            valoresObjetivo[i] = objetivoInt / 100f;

            if (textosValorObjetivo[i] != null)
                textosValorObjetivo[i].text = objetivoInt.ToString();
        }
    }

    private void ConfigurarSliders()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            int idx = i; 
            sliders[i].onValueChanged.AddListener((valor) => ActualizarTextoSlider(idx, valor));
            sliders[i].value = 0f;
        }
    }

    private void ActualizarTextoSlider(int index, float valor)
    {
        if (textosValorActual[index] != null)
        {
            int valorInt = Mathf.RoundToInt(valor * 100f);
            textosValorActual[index].text = valorInt.ToString();
        }
    }

    private void ActualizarTextosValoresActuales()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            if (textosValorActual[i] != null)
            {
                int valorInt = Mathf.RoundToInt(sliders[i].value * 100f);
                textosValorActual[i].text = valorInt.ToString();
            }
        }
    }

    private void ResetearUI()
    {
        foreach (Slider s in sliders)
            s.value = 0f;

        tiempoCorrecto = 0f;
        feedbackText.text = "Ajusta las frecuencias";
        textoInstrucciones.text = "Mueve cada barra hasta alcanzar el valor indicado";

        SetCartelAlpha(0f);
        cartelOnAir.gameObject.SetActive(true);
    }

    private void VerificarSeñal()
    {
        bool todoCorrecto = true;

        for (int i = 0; i < sliders.Length; i++)
        {
            if (Mathf.Abs(sliders[i].value - valoresObjetivo[i]) > tolerancia)
            {
                todoCorrecto = false;
                break;
            }
        }

        if (todoCorrecto)
        {
            tiempoCorrecto += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempoCorrecto / tiempoRequerido);
            SetCartelAlpha(progreso);

            float tiempoRestante = tiempoRequerido - tiempoCorrecto;
            feedbackText.text = $"¡Señal clara! Mantén {tiempoRestante:F1}s";

            if (tiempoCorrecto >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoCorrecto = 0f;
            SetCartelAlpha(0f);
            feedbackText.text = "Ajusta las barras hasta que coincidan con los valores objetivo";
        }
    }

    private void SetCartelAlpha(float alpha)
    {
        Color c = cartelOnAir.color;
        c.a = alpha;
        cartelOnAir.color = c;
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        SetCartelAlpha(1f);
        feedbackText.text = "¡AL AIRE!";

        yield return new WaitForSeconds(1.5f);

        if (GuideManager.Instance != null)
            GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinRadio);
        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.CompleteMinigame(minigameIndex);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}