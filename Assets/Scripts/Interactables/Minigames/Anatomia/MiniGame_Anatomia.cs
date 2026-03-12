using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGame_Anatomia : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoResultado;

    [Header("Configuración")]
    [SerializeField] private int idMinijuego;
    [SerializeField] private int totalOrganos = 6;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private int organosColocados = 0;
    private bool juegoCompletado = false;

    private void Start()
    {
        ActualizarUI();
        textoResultado.gameObject.SetActive(false);
    }

    public void OrganoColocado()
    {
        if (juegoCompletado) return;

        organosColocados++;
        textoPuntuacion.text = $"{organosColocados}/{totalOrganos}";

        if (organosColocados >= totalOrganos)
        {
            StartCoroutine(GanarJuego());
        }
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;

        textoInstrucciones.text = "¡COMPLETADO!";
        textoResultado.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        GameProgressManager.Instance.CompletarMinijuego(idMinijuego);

        Time.timeScale = 1f;

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoPuntuacion.text = $"0/{totalOrganos}";
        textoInstrucciones.text = "Arrastra cada órgano a su lugar";
        textoResultado.gameObject.SetActive(false);
    }
}