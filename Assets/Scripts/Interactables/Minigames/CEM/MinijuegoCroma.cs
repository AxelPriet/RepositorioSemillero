using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinijuegoCroma : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Image luz1, luz2, luz3; // luces a ajustar
    public Slider sliderR, sliderG, sliderB;
    public TextMeshProUGUI feedbackText;

    public void CheckColores()
    {
        // ejemplo simple: si los sliders se acercan a un valor target
        bool correcto = Mathf.Abs(sliderR.value - 0.8f) < 0.1f &&
                         Mathf.Abs(sliderG.value - 0.3f) < 0.1f &&
                         Mathf.Abs(sliderB.value - 0.2f) < 0.1f;

        if (correcto)
        {
            feedbackText.text = "¡Colores correctos!";
            CompleteSubMinigame();
        }
        else
        {
            feedbackText.text = "Intenta de nuevo";
            FailSubMinigame();
        }
    }

    public void CompleteSubMinigame()
    {
        panel.SetActive(false);
    }

    public void FailSubMinigame()
    {
        panel.SetActive(false);
    }
}

