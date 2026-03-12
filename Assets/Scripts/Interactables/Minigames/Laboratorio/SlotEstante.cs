using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotEstante : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int capacidadTotal = 2;
    [SerializeField] private int espacioUsado = 0;

    private Image imagenSlot;
    private TextMeshProUGUI textoCapacidad;
    private List<Implemento> implementos = new List<Implemento>();

    private void Awake()
    {
        imagenSlot = GetComponent<Image>();
        textoCapacidad = GetComponentInChildren<TextMeshProUGUI>();

        if (imagenSlot != null)
            imagenSlot.color = new Color(1, 1, 1, 0.3f);

        // Crear texto si no existe
        if (textoCapacidad == null)
        {
            GameObject textoObj = new GameObject("TextoCapacidad");
            textoObj.transform.SetParent(transform, false);
            textoCapacidad = textoObj.AddComponent<TextMeshProUGUI>();
            textoCapacidad.fontSize = 24;
            textoCapacidad.alignment = TextAlignmentOptions.Center;
            textoCapacidad.color = Color.white;

            RectTransform rect = textoObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 50);
            rect.anchoredPosition = Vector2.zero;
        }

        ActualizarTexto();
    }

    public bool EspacioDisponible(int tamañoImplemento)
    {
        return (espacioUsado + tamañoImplemento) <= capacidadTotal;
    }

    public void AgregarImplemento(Implemento implemento)
    {
        if (!EspacioDisponible(implemento.Tamaño)) return;

        implementos.Add(implemento);
        espacioUsado += implemento.Tamaño;

        if (imagenSlot != null)
        {
            float proporcion = (float)espacioUsado / capacidadTotal;
            imagenSlot.color = Color.Lerp(new Color(1, 1, 1, 0.3f), new Color(0, 1, 0, 0.3f), proporcion);
        }

        ActualizarTexto();
    }

    public void RemoverImplemento(Implemento implemento)
    {
        if (implementos.Contains(implemento))
        {
            implementos.Remove(implemento);
            espacioUsado -= implemento.Tamaño;

            if (imagenSlot != null)
            {
                float proporcion = (float)espacioUsado / capacidadTotal;
                imagenSlot.color = Color.Lerp(new Color(1, 1, 1, 0.3f), new Color(0, 1, 0, 0.3f), proporcion);
            }

            ActualizarTexto();
        }
    }

    private void ActualizarTexto()
    {
        if (textoCapacidad != null)
            textoCapacidad.text = $"{espacioUsado}/{capacidadTotal}";
    }

    public bool EstaLleno()
    {
        return espacioUsado >= capacidadTotal;
    }

    public int EspacioUsado
    {
        get { return espacioUsado; }
    }

    public int CapacidadTotal
    {
        get { return capacidadTotal; }
    }
}