using UnityEngine;
using UnityEngine.UI;

public class OndaVisualSimple : MonoBehaviour
{
    [SerializeField] private Image imagenOnda;
    [SerializeField] private float amplitud = 10f;
    [SerializeField] private float velocidad = 2f;

    private void Update()
    {
        if (imagenOnda == null) return;

        // Crear una onda simple moviendo la posición Y
        float onda = Mathf.Sin(Time.time * velocidad) * amplitud;

        // Aplicar a la imagen (puedes modificar la rotación, escala o posición)
        Vector3 pos = imagenOnda.rectTransform.anchoredPosition;
        pos.y = onda;
        imagenOnda.rectTransform.anchoredPosition = pos;
    }

    public void SetCalidad(float calidad)
    {
        // Cambiar color o amplitud según la calidad
        if (calidad > 0.8f)
        {
            imagenOnda.color = Color.green;
            amplitud = 15f;
        }
        else
        {
            imagenOnda.color = Color.red;
            amplitud = 5f + (calidad * 10f);
        }
    }
}