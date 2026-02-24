using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinijuegoRadio : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Slider perilla1;
    public Slider perilla2;
    public Image señal;
    public TextMeshProUGUI feedbackText;
    public void StartMinijuego()
    {

    }
    public void CheckSeñal()
    {
        // ejemplo simple: si sliders cerca de 0.5 es correcto
        bool correcto = Mathf.Abs(perilla1.value - 0.5f) < 0.1f &&
                        Mathf.Abs(perilla2.value - 0.5f) < 0.1f;

        if (correcto)
        {
            señal.color = Color.green;
            feedbackText.text = "Señal clara!";
            CompleteSubMinigame();
        }
        else
        {
            señal.color = Color.red;
            feedbackText.text = "Señal incorrecta, reintenta";
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