using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotEstante : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int capacidadTotal = 3;
    [SerializeField] private int espacioUsado = 0;

    private Image imagenSlot;
    private TextMeshProUGUI textoCapacidad;
    private Implemento implementoActual = null;

    private void Awake()
    {
        imagenSlot = GetComponent<Image>();
        textoCapacidad = GetComponentInChildren<TextMeshProUGUI>();

        if (imagenSlot != null)
            imagenSlot.color = new Color(1, 1, 1, 0.3f);

        ActualizarTexto();
    }

    public bool EspacioDisponible(int tamañoImplemento)
    {
        return (espacioUsado + tamañoImplemento) <= capacidadTotal;
    }

    public void Ocupar(int tamañoImplemento, Implemento implemento)
    {
        espacioUsado += tamañoImplemento;
        implementoActual = implemento;

        if (imagenSlot != null)
            imagenSlot.color = new Color(0, 1, 0, 0.3f); 

        ActualizarTexto();
    }

    public void Liberar()
    {
        if (implementoActual != null)
        {
            espacioUsado -= implementoActual.Tamaño;
            implementoActual = null;

            if (imagenSlot != null)
                imagenSlot.color = new Color(1, 1, 1, 0.3f);

            ActualizarTexto();
        }
    }

    private void ActualizarTexto()
    {
        if (textoCapacidad != null)
            textoCapacidad.text = $"{espacioUsado}/{capacidadTotal}";
    }

    public bool EstaOcupado()
    {
        return implementoActual != null;
    }
}