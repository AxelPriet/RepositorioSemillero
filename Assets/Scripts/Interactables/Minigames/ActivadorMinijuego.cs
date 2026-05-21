using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ActivadorMinijuego : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Minijuego")]
    [SerializeField] private string nombreEscena;
    [SerializeField] private string mensajePrompt = "Iniciar Minijuego";
    [SerializeField] private int idProgreso;

    private bool minijuegoActivo = false;

    private void Start()
    {
    }

    public void Interactuar()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;
        if (minijuegoActivo) return;

        minijuegoActivo = true;
        PlayerManager.Instance.OcultarJugador();
        TransicionEscenas.Instance.CargarEscena(nombreEscena);
    }

    public string GetPrompt()
    {
        return mensajePrompt;
    }

    public bool PuedeInteractuar()
    {
        if (minijuegoActivo) return false;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return false;
        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            if (PlayerManager.Instance != null)
                PlayerManager.Instance.MostrarJugador();

            minijuegoActivo = false;
        }
    }
}