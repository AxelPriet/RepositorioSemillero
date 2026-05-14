using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinijuegoTina : MonoBehaviour
{
    [Header("Agua de la Tina")]
    [SerializeField] private Image aguaTina;
    [SerializeField] private Gradient gradienteTemperatura;

    [Header("Termómetro (indicador de línea)")]
    [SerializeField] private RectTransform termometroIndicador; 
    [SerializeField] private float alturaMaximaTermometro = 200f; 

    [Header("Temperatura")]
    [SerializeField] private float temperaturaMin = 0f;
    [SerializeField] private float temperaturaMax = 100f;
    [SerializeField] private float temperaturaOptimaMin = 40f;
    [SerializeField] private float temperaturaOptimaMax = 60f;

    [Header("Llaves")]
    [SerializeField] private float velocidadSubida = 15f;  
    [SerializeField] private float velocidadBajada = 15f;   
    [SerializeField] private float enfriamientoNatural = 5f; 

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoTemperatura;
    [SerializeField] private TextMeshProUGUI textoTemporizador;

    [Header("Configuración")]
    [SerializeField] private float tiempoRequerido = 15f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PlayerControls playerControls;
    private float temperaturaActual = 30f;
    private float tiempoAcumulado = 0f;
    private bool minijuegoCompletado = false;
    [SerializeField] private int minigameIndex;

    private void Start()
    {
        playerControls = InputHandler.Instance.GetControls();
        ActualizarUI();
    }

    private void Update()
    {
        if (minijuegoCompletado) return;

        ActualizarTemperatura();
        ActualizarVisuales();
        VerificarRangoOptimo();
    }

    private void ActualizarTemperatura()
    {
        float cambio = 0f;

        bool subir = Keyboard.current.aKey.isPressed;
        bool bajar = Keyboard.current.dKey.isPressed;

        if (subir)
            cambio += velocidadSubida * Time.deltaTime;
        if (bajar)
            cambio -= velocidadBajada * Time.deltaTime;

        if (!subir && !bajar)
        {
            cambio -= enfriamientoNatural * Time.deltaTime;
        }

        temperaturaActual += cambio;
        temperaturaActual = Mathf.Clamp(temperaturaActual, temperaturaMin, temperaturaMax);

        textoTemperatura.text = $"{temperaturaActual:F1}°C";
    }

    private void ActualizarVisuales()
    {
        float proporcion = (temperaturaActual - temperaturaMin) / (temperaturaMax - temperaturaMin);

        if (termometroIndicador != null)
        {
            float nuevaAltura = proporcion * alturaMaximaTermometro;
            nuevaAltura = Mathf.Clamp(nuevaAltura, 0f, alturaMaximaTermometro);
            termometroIndicador.sizeDelta = new Vector2(termometroIndicador.sizeDelta.x, nuevaAltura);
        }

        if (aguaTina != null)
        {
            float t = (temperaturaActual - temperaturaMin) / (temperaturaMax - temperaturaMin);
            aguaTina.color = gradienteTemperatura.Evaluate(t);
        }
    }

    private void VerificarRangoOptimo()
    {
        bool enRango = temperaturaActual >= temperaturaOptimaMin && temperaturaActual <= temperaturaOptimaMax;

        if (enRango)
        {
            tiempoAcumulado += Time.deltaTime;
            float restante = tiempoRequerido - tiempoAcumulado;
            textoTemporizador.text = $"{restante:F1}s";

            if (tiempoAcumulado >= tiempoRequerido)
                StartCoroutine(CompletarMinijuego());
        }
        else
        {
            tiempoAcumulado = Mathf.Max(0, tiempoAcumulado - Time.deltaTime * 2f);
            textoTemporizador.text = $"{tiempoRequerido - tiempoAcumulado:F1}s";
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        yield return new WaitForSeconds(1.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinFisioterapia);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoTemperatura.text = $"{temperaturaActual:F1}°C";
        textoTemporizador.text = $"{tiempoRequerido:F0}s";
    }
}