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
    [SerializeField] private bool llaveFriaAbierta = false;
    [SerializeField] private bool llaveCalienteAbierta = false;
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

        ProcesarInput();
        ActualizarTemperatura();
        ActualizarVisuales();
        VerificarRangoOptimo();
    }

    private void ProcesarInput()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            llaveFriaAbierta = !llaveFriaAbierta;
            Debug.Log($"Llave fría: {(llaveFriaAbierta ? "ABIERTA" : "CERRADA")}");
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            llaveCalienteAbierta = !llaveCalienteAbierta;
            Debug.Log($"Llave caliente: {(llaveCalienteAbierta ? "ABIERTA" : "CERRADA")}");
        }
    }
    private void ActualizarTemperatura()
    {
        float cambio = 0f;

        if (llaveFriaAbierta)
            cambio += efectoFrio * Time.deltaTime;
        if (llaveCalienteAbierta)
            cambio += efectoCalor * Time.deltaTime;

        if (!llaveFriaAbierta && !llaveCalienteAbierta)
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

        float alturaFrio = proporcionFrio * alturaMaxima;
        float alturaCalor = proporcionCalor * alturaMaxima;

        nivelFrio.sizeDelta = new Vector2(nivelFrio.sizeDelta.x, alturaFrio);
        nivelCalor.sizeDelta = new Vector2(nivelCalor.sizeDelta.x, alturaCalor);

        if (aguaTina != null)
        {
            float t = (temperaturaActual - temperaturaMin) / (temperaturaMax - temperaturaMin);
            aguaTina.color = gradienteTemperatura.Evaluate(t);
        }
    }

    private void VerificarRangoOptimo()
    {
        bool enRango = temperaturaActual >= temperaturaOptimaMin &&
                      temperaturaActual <= temperaturaOptimaMax;

        if (enRango)
        {
            tiempoAcumulado += Time.deltaTime;
            float restante = tiempoRequerido - tiempoAcumulado;
            textoTemporizador.text = $"{restante:F1}s";

            if (tiempoAcumulado >= tiempoRequerido)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            tiempoAcumulado = Mathf.Max(0, tiempoAcumulado - Time.deltaTime * 2f);
            float restante = tiempoRequerido - tiempoAcumulado;
            textoTemporizador.text = $"{restante:F1}s";
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
        textoInstrucciones.text = "A: Agua fría | D: Agua caliente\nMantén la temperatura entre 40°C y 60°C";
        textoTemperatura.text = $"{temperaturaActual:F1}°C";
        textoTemporizador.text = $"{tiempoRequerido:F0}s";
    }
}