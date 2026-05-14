using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniGame_Barras : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int totalBarras = 4;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoBarras;

    private int barrasColocadas = 0;
    private bool juegoCompletado = false;
    private List<string> barrasColocadasList = new List<string>();
    [SerializeField] private int minigameIndex;

    private void Start()
    {
        ActualizarTexto();
    }

    public void BarraColocada(string nombreBarra)
    {
        if (juegoCompletado) return;

        if (!barrasColocadasList.Contains(nombreBarra))
        {
            barrasColocadasList.Add(nombreBarra);
            barrasColocadas++;
            ActualizarTexto();

            if (barrasColocadas >= totalBarras)
                StartCoroutine(CompletarJuego());
        }
    }

    private void ActualizarTexto()
    {
        if (textoBarras != null)
            textoBarras.text = $"Barras: {barrasColocadas}/{totalBarras}";
    }

    private IEnumerator CompletarJuego()
    {
        juegoCompletado = true;

        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinCensei);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}