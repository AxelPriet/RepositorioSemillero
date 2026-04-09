using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinijuegoRadio : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform perilla1;
    [SerializeField] private RectTransform perilla2;
    [SerializeField] private Image señal;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private float tolerancia = 0.1f;
    [SerializeField] private float tiempoRequerido = 3f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PerillaRadio perilla1Script;
    private PerillaRadio perilla2Script;
    private float valorObjetivoPerilla1;
    private float valorObjetivoPerilla2;
    private float tiempoCorrecto = 0f;
    private bool minijuegoCompletado = false;
    [SerializeField] private int minigameIndex;

    private void Awake()
    {
        perilla1Script = perilla1.GetComponent<PerillaRadio>();
        perilla2Script = perilla2.GetComponent<PerillaRadio>();
    }

    private void Start()
    {
        GenerarValoresAleatorios();
        ResetearUI();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;
        VerificarSeñal();
    }

    private void GenerarValoresAleatorios()
    {
        valorObjetivoPerilla1 = Random.Range(0.2f, 0.8f);
        valorObjetivoPerilla2 = Random.Range(0.2f, 0.8f);
    }

    private void ResetearUI()
    {
        perilla1Script.ResetearPerilla();
        perilla2Script.ResetearPerilla();
        señal.color = Color.red;
        feedbackText.text = "Ajusta las perillas";
        textoInstrucciones.text = "Gira las perillas para sintonizar";
    }

    private void VerificarSeñal()
    {
        float diferencia1 = Mathf.Abs(perilla1Script.ValorNormalizado - valorObjetivoPerilla1);
        float diferencia2 = Mathf.Abs(perilla2Script.ValorNormalizado - valorObjetivoPerilla2);

        bool perilla1Correcta = diferencia1 < tolerancia;
        bool perilla2Correcta = diferencia2 < tolerancia;
        bool todoCorrecto = perilla1Correcta && perilla2Correcta;

        if (todoCorrecto)
        {
            tiempoCorrecto += Time.deltaTime;
            float tiempoRestante = tiempoRequerido - tiempoCorrecto;

            feedbackText.text = $"Mantén la señal: {tiempoRestante:F1}s";
            señal.color = Color.Lerp(Color.yellow, Color.green, tiempoCorrecto / tiempoRequerido);

            if (tiempoCorrecto >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoCorrecto = 0f;

            if (!perilla1Correcta && !perilla2Correcta)
                feedbackText.text = "Ajusta ambas perillas";
            else if (!perilla1Correcta)
                feedbackText.text = "Ajusta perilla izquierda";
            else if (!perilla2Correcta)
                feedbackText.text = "Ajusta perilla derecha";

            señal.color = Color.red;
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡SEÑAL CLARA!";
        señal.color = Color.green;

        yield return new WaitForSeconds(1f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinRadio);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}