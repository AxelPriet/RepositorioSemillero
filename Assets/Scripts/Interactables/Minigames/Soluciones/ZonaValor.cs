using UnityEngine;
using UnityEngine.UI;

public class ZonaValor : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string instrumentoEsperado; // "Pipeta", "Probeta", etc.

    private Image imagen;
    private Color colorOriginal;
    private bool ocupada = false;

    private void Awake()
    {
        imagen = GetComponent<Image>();
        if (imagen != null)
        {
            colorOriginal = imagen.color;
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);
        }
    }

    public bool IntentarColocar(string instrumento)
    {
        if (ocupada) return false;

        if (instrumento == instrumentoEsperado)
        {
            ocupada = true;
            return true;
        }
        return false;
    }

    public void IndicarSeleccion(bool seleccionada)
    {
        if (ocupada || imagen == null) return;

        if (seleccionada)
            imagen.color = Color.yellow;
        else
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);
    }

    public void IndicarCompletado()
    {
        if (imagen != null)
            imagen.color = Color.green;
    }

    public bool EstaOcupada()
    {
        return ocupada;
    }

    public void Resetear()
    {
        ocupada = false;
        if (imagen != null)
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);
    }
}