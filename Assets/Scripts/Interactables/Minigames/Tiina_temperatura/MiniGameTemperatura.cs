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

    [Header("Termómetros de Nivel")]
    [SerializeField] private RectTransform nivelFrio;
    [SerializeField] private RectTransform nivelCalor;
    [SerializeField] private float alturaMaxima = 200f;

    [Header("Temperatura")]
    [SerializeField] private float temperaturaMin = 0f;
    [SerializeField] private float temperaturaMax = 100f;
    [SerializeField] private float temperaturaOptimaMin = 40f;
    [SerializeField] private float temperaturaOptimaMax = 60f;

    [Header("Llaves")]
    [SerializeField] private float efectoFrio = -8f;
    [SerializeField] private float efectoCalor = 8f;
    [SerializeField] private float enfriamientoNatural = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoTemperatura;
    [SerializeField] private TextMeshProUGUI textoTemporizador;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private float tiempoRequerido = 15f;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private PlayerControls playerControls;
    private float temperaturaActual = 30f;
    private float tiempoAcumulado = 0f;
    private bool minijuegoCompletado = false;

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

        bool friaPresionada = Keyboard.current.aKey.isPressed;
        bool calientePresionada = Keyboard.current.dKey.isPressed;

        if (friaPresionada)
            cambio += efectoFrio * Time.deltaTime;
        if (calientePresionada)
            cambio += efectoCalor * Time.deltaTime;

        if (!friaPresionada && !calientePresionada)
        {
            if (temperaturaActual > 30f)
                cambio = -enfriamientoNatural * Time.deltaTime;
            else if (temperaturaActual < 30f)
                cambio = enfriamientoNatural * Time.deltaTime;
        }

        temperaturaActual += cambio;
        temperaturaActual = Mathf.Clamp(temperaturaActual, temperaturaMin, temperaturaMax);

        textoTemperatura.text = $"{temperaturaActual:F1}°C";
    }

    private void ActualizarVisuales()
    {
        float proporcionFrio = Mathf.Clamp01((temperaturaMax - temperaturaActual) / (temperaturaMax - temperaturaMin));
        float proporcionCalor = Mathf.Clamp01(temperaturaActual / temperaturaMax);

        nivelFrio.sizeDelta = new Vector2(nivelFrio.sizeDelta.x, proporcionFrio * alturaMaxima);
        nivelCalor.sizeDelta = new Vector2(nivelCalor.sizeDelta.x, proporcionCalor * alturaMaxima);

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
        textoInstrucciones.text = "¡TEMPERATURA PERFECTA!";
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoInstrucciones.text = "Mantén A (fría) o D (caliente)\nObjetivo: 40°C – 60°C";
        textoTemperatura.text = $"{temperaturaActual:F1}°C";
        textoTemporizador.text = $"{tiempoRequerido:F0}s";
    }
}