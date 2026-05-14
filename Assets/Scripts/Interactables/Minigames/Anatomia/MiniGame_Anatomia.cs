using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGame_Anatomia : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoFaltantes;

    [Header("Configuración")]
    [SerializeField] private int totalOrganos = 6;
    [SerializeField] private string nombreEscenaPrincipal = "Main";
    [SerializeField] private int minigameIndex;

    private int organosColocados = 0;
    private bool juegoCompletado = false;

    private void Start()
    {
        ActualizarUI();
    }

    public void OrganoColocado()
    {
        if (juegoCompletado) return;

        organosColocados++;
        int faltantes = totalOrganos - organosColocados;
        textoFaltantes.text = $"Faltan: {faltantes}";

        if (organosColocados >= totalOrganos)
        {
            StartCoroutine(GanarJuego());
        }
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;
        textoFaltantes.text = "¡COMPLETADO!";
        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinAnfiteatro);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoFaltantes.text = $"Faltan: {totalOrganos}";
    }
}