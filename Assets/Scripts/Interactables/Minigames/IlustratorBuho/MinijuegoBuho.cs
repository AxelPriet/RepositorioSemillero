using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinijuegoBuho : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;

    [Header("Configuración")]
    [SerializeField] private int totalPiezas = 6;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private int piezasColocadas = 0;
    private bool minijuegoCompletado = false;
    [SerializeField] private int minigameIndex;


    private void Start()
    {
        textoPuntuacion.text = $"0/{totalPiezas}";
    }

    public void PiezaColocada()
    {
        if (minijuegoCompletado) return;

        piezasColocadas++;
        textoPuntuacion.text = $"{piezasColocadas}/{totalPiezas}";

        if (piezasColocadas >= totalPiezas)
        {
            StartCoroutine(CompletarMinijuego());
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        yield return new WaitForSeconds(1.5f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinSalaMac);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}