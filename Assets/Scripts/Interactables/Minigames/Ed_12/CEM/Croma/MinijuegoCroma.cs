using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinijuegoCroma : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private PerillaRadio perillaRoja;
    [SerializeField] private PerillaRadio perillaVerde;
    [SerializeField] private PerillaRadio perillaAzul;
    [SerializeField] private Image luzRoja;
    [SerializeField] private Image luzVerde;
    [SerializeField] private Image luzAzul;
    [SerializeField] private Image colorObjetivo;
    [SerializeField] private Image colorActual;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private float tolerancia = 0.1f;
    [SerializeField] private float tiempoRequerido = 2f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private float valorObjetivoRojo;
    private float valorObjetivoVerde;
    private float valorObjetivoAzul;
    private float tiempoCorrecto = 0f;
    private bool minijuegoCompletado = false;
    [SerializeField] private int minigameIndex;


    private void Start()
    {
        GenerarColorObjetivo();
        ResetearPerillas();
        ActualizarUI();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ActualizarLuces();
        ActualizarColorMezclado();
        VerificarMezcla();
    }

    private void GenerarColorObjetivo()
    {
        valorObjetivoRojo = Random.Range(0.2f, 0.8f);
        valorObjetivoVerde = Random.Range(0.2f, 0.8f);
        valorObjetivoAzul = Random.Range(0.2f, 0.8f);
        colorObjetivo.color = new Color(valorObjetivoRojo, valorObjetivoVerde, valorObjetivoAzul);
    }

    private void ResetearPerillas()
    {
        perillaRoja.ResetearPerilla();
        perillaVerde.ResetearPerilla();
        perillaAzul.ResetearPerilla();
    }

    private void ActualizarUI()
    {
        feedbackText.text = "Iguala el color objetivo";
        textoInstrucciones.text = "Ajusta las perillas RGB";
    }

    private void ActualizarLuces()
    {
        luzRoja.color = Color.red * perillaRoja.ValorNormalizado;
        luzVerde.color = Color.green * perillaVerde.ValorNormalizado;
        luzAzul.color = Color.blue * perillaAzul.ValorNormalizado;
    }

    private void ActualizarColorMezclado()
    {
        float r = perillaRoja.ValorNormalizado;
        float g = perillaVerde.ValorNormalizado;
        float b = perillaAzul.ValorNormalizado;
        colorActual.color = new Color(r, g, b);
    }

    private void VerificarMezcla()
    {
        bool rojoCorrecto = Mathf.Abs(perillaRoja.ValorNormalizado - valorObjetivoRojo) < tolerancia;
        bool verdeCorrecto = Mathf.Abs(perillaVerde.ValorNormalizado - valorObjetivoVerde) < tolerancia;
        bool azulCorrecto = Mathf.Abs(perillaAzul.ValorNormalizado - valorObjetivoAzul) < tolerancia;
        bool todoCorrecto = rojoCorrecto && verdeCorrecto && azulCorrecto;

        if (todoCorrecto)
        {
            tiempoCorrecto += Time.deltaTime;
            float tiempoRestante = tiempoRequerido - tiempoCorrecto;

            feedbackText.text = $"¡Color perfecto! Mantén {tiempoRestante:F1}s";
            colorActual.transform.localScale = Vector3.one * (1f + (tiempoCorrecto / tiempoRequerido * 0.2f));

            if (tiempoCorrecto >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoCorrecto = 0f;
            colorActual.transform.localScale = Vector3.one;

            if (!rojoCorrecto && !verdeCorrecto && !azulCorrecto)
                feedbackText.text = "Ajusta todos los colores";
            else if (!rojoCorrecto && !verdeCorrecto)
                feedbackText.text = "Ajusta rojo y verde";
            else if (!rojoCorrecto && !azulCorrecto)
                feedbackText.text = "Ajusta rojo y azul";
            else if (!verdeCorrecto && !azulCorrecto)
                feedbackText.text = "Ajusta verde y azul";
            else if (!rojoCorrecto)
                feedbackText.text = "Ajusta rojo";
            else if (!verdeCorrecto)
                feedbackText.text = "Ajusta verde";
            else if (!azulCorrecto)
                feedbackText.text = "Ajusta azul";
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        feedbackText.text = "¡CROMA PERFECTO!";

        float tiempo = 0;
        while (tiempo < 0.5f)
        {
            colorActual.transform.localScale = Vector3.one * (1.2f + Mathf.Sin(tiempo * 20f) * 0.1f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinCroma);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}