using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivadorMinijuego : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Minijuego")]
    [SerializeField] private string nombreEscena; 
    [SerializeField] private string mensajePrompt = "Iniciar Minijuego";
    [SerializeField] private int idProgreso; 

    private bool minijuegoActivo = false;

    public void Interactuar()
    {
        if (minijuegoActivo) return;

        minijuegoActivo = true;

        PlayerManager.Instance.OcultarJugador();

        SceneManager.LoadScene(nombreEscena, LoadSceneMode.Single);
    }

    public string GetPrompt()
    {
        return mensajePrompt;
    }

    public bool PuedeInteractuar()
    {
        return !minijuegoActivo;
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
        }
    }
}