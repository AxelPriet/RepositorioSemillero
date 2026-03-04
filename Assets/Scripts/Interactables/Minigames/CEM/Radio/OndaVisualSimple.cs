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

        float onda = Mathf.Sin(Time.time * velocidad) * amplitud;
        Vector3 pos = imagenOnda.rectTransform.anchoredPosition;
        pos.y = onda;
        imagenOnda.rectTransform.anchoredPosition = pos;
    }
}