using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGame_Libros : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoResultado;

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
        textoResultado.gameObject.SetActive(false);
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
        textoPuntuacion.text = $"Libros: {librosColocados}/{totalLibros}";

        if (librosColocados >= totalLibros)
        {
            StartCoroutine(GanarJuego());
        }
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;
        juegoActivo = false;

        textoInstrucciones.text = "¡ESTANTERÍA ORDENADA!";
        textoPuntuacion.gameObject.SetActive(false);
        textoTiempo.gameObject.SetActive(false);

        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡COMPLETASTE!";

        yield return new WaitForSeconds(2f);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private IEnumerator PerderJuego()
    {
        juegoCompletado = true;
        juegoActivo = false;

        textoInstrucciones.text = "¡TIEMPO AGOTADO!";
        textoPuntuacion.gameObject.SetActive(false);
        textoTiempo.gameObject.SetActive(false);

        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡PERDISTE!";
        textoResultado.color = Color.red;

        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinPoliteca);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoTiempo.text = $"Tiempo: {tiempoLimite:F0}s";
        textoPuntuacion.text = $"Libros: 0/{totalLibros}";
        textoInstrucciones.text = "Ordena los libros de claro a oscuro";
        textoResultado.gameObject.SetActive(false);
    }
}