using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinijuegoLaboratorio : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoEstado;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private int totalInstrumentos = 4;
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";
    [SerializeField] private int minigameIndex;


    private int aciertos = 0;
    private bool juegoCompletado = false;

    private void Start()
    {
        textoEstado.text = $"0/{totalInstrumentos}";
        textoInstrucciones.text = "Arrastra cada instrumento a su zona";
    }

    public void InstrumentoColocado()
    {
        if (juegoCompletado) return;

        aciertos++;
        textoEstado.text = $"{aciertos}/{totalInstrumentos}";

        if (aciertos >= totalInstrumentos)
        {
            StartCoroutine(CompletarJuego());
        }
    }

    private IEnumerator CompletarJuego()
    {
        juegoCompletado = true;
        textoInstrucciones.text = "¡LO LOGRASTE!";
        textoEstado.text = "¡COMPLETADO!";

        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinLaboratorioSoluciones);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}