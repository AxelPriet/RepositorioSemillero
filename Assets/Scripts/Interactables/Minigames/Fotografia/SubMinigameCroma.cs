using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubMinigameCroma : MonoBehaviour, ISubMinigame
{
    [Header("UI")]
    public GameObject panel;
    public Image luz1, luz2, luz3; // luces a ajustar
    public Slider sliderR, sliderG, sliderB;
    public TextMeshProUGUI feedbackText;

    private MinigameCombinado parent;

    public void StartSubMinigame(MinigameCombinado parent)
    {
        this.parent = parent;
        panel.SetActive(true);
        feedbackText.text = "Ajusta las luces para que concuerden los colores";

        // inicializar sliders si quieres
        sliderR.value = 0;
        sliderG.value = 0;
        sliderB.value = 0;
    }

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
        parent.OnSubMinijuegoComplete();
    }

    public void FailSubMinigame()
    {
        panel.SetActive(false);
        parent.OnSubMinijuegoFail();
    }
}

