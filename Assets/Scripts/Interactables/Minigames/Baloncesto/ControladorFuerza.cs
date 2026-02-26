using UnityEngine;
using UnityEngine.UI;

public class ControladorFuerza : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Slider sliderFuerza;
    [SerializeField] private float fuerzaMin = 5f;
    [SerializeField] private float fuerzaMax = 15f;
    [SerializeField] private float velocidadSlider = 2f;

    private float direccion = 1f;

    public float ObtenerFuerzaActual()
    {
        return Mathf.Lerp(fuerzaMin, fuerzaMax, sliderFuerza.value);
    }

    public void Actualizar()
    {
        sliderFuerza.value += direccion * velocidadSlider * Time.deltaTime;

        if (sliderFuerza.value >= sliderFuerza.maxValue)
        {
            sliderFuerza.value = sliderFuerza.maxValue;
            direccion = -1f;
        }
        else if (sliderFuerza.value <= sliderFuerza.minValue)
        {
            sliderFuerza.value = sliderFuerza.minValue;
            direccion = 1f;
        }
    }

    public void IniciarMovimiento()
    {
        direccion = 1f;
    }

    public void Resetear()
    {
        sliderFuerza.value = 0.5f;
        direccion = 1f;
    }
}
