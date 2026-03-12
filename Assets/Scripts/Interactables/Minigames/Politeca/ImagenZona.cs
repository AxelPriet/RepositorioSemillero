using UnityEngine;
using UnityEngine.UI;

public class ImagenZona2 : MonoBehaviour
{
    private Image imagen;
    private Color colorOriginal;

    private void Awake()
    {
        imagen = GetComponent<Image>();
        if (imagen != null)
        {
            colorOriginal = imagen.color;
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);
        }
    }

    public void Destacar()
    {
        if (imagen != null)
            imagen.color = Color.yellow;
    }

    public void Completado()
    {
        if (imagen != null)
            imagen.color = Color.green;
    }

    public void ResetearColor()
    {
        if (imagen != null)
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);
    }
}