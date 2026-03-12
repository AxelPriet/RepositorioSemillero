using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivadorMinijuego : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Minijuego")]
    [SerializeField] private string nombreEscena;
    [SerializeField] private string mensajePrompt = "Iniciar Minijuego";
    [SerializeField] private int idMinijuego; 

    private bool minijuegoActivo = false;

    public void Interactuar()
    {
        if (minijuegoActivo) return;

        if (GameProgressManager.Instance.EstaCompletado(idMinijuego))
            return;

        minijuegoActivo = true;

        PlayerManager.Instance?.OcultarJugador();

        Time.timeScale = 1f;

        SceneManager.LoadSceneAsync(nombreEscena, LoadSceneMode.Single);
    }

    public string GetPrompt()
    {
        if (GameProgressManager.Instance.EstaCompletado(idMinijuego))
            return "Completado ✓";

        return mensajePrompt;
    }

    public bool PuedeInteractuar()
    {
        return !minijuegoActivo && GameProgressManager.Instance.PuedeJugar(idMinijuego);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}