using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGame_Libros : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoTiempo;

    [Header("Configuración")]
    [SerializeField] private float tiempoLimite = 30f;
    [SerializeField] private int totalLibros = 7;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private float tiempoRestante;
    private int librosColocados = 0;
    private bool juegoActivo = false;
    private bool juegoCompletado = false;
    private bool temporizadorActivo = false;
    [SerializeField] private int minigameIndex;


    private void Start()
    {
        InicializarJuego();
    }

    private void InicializarJuego()
    {
        tiempoRestante = tiempoLimite;
        librosColocados = 0;
        juegoActivo = true;
        temporizadorActivo = false;
        juegoCompletado = false;

        ActualizarUI();
    }

    private void Update()
    {
        if (!juegoActivo || juegoCompletado) return;

        if (temporizadorActivo)
        {
            tiempoRestante -= Time.deltaTime;
            textoTiempo.text = $"Tiempo: {tiempoRestante:F1}s";

            if (tiempoRestante <= 0)
            {
                StartCoroutine(PerderJuego());
            }
        }
    }

    public void LibroColocadoCorrectamente()
    {
        if (!juegoActivo || juegoCompletado) return;

        if (!temporizadorActivo)
        {
            temporizadorActivo = true;
        }

        librosColocados++;

        if (librosColocados >= totalLibros)
        {
            StartCoroutine(GanarJuego());
        }
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;
        juegoActivo = false;

        textoTiempo.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private IEnumerator PerderJuego()
    {
        juegoCompletado = true;
        juegoActivo = false;

        textoTiempo.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinPoliteca);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoTiempo.text = $"Tiempo: {tiempoLimite:F0}s";
    }
}