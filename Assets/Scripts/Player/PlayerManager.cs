using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private GameObject jugadorActual;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(BuscarJugadorInicial());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private IEnumerator BuscarJugadorInicial()
    {
        yield return null;
        BuscarJugador();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Escena cargada: {scene.name}");
        Time.timeScale = 1f;
        StartCoroutine(ReactivarJugadorProximoFrame(scene));
    }

    private IEnumerator ReactivarJugadorProximoFrame(Scene scene)
    {
        yield return null; // Esperar un frame
        yield return null; // Esperar otro frame para asegurar

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugadorActual = player;
            playerMovement = player.GetComponent<PlayerMovement>();

            // FORZAR ACTIVACIÓN
            jugadorActual.SetActive(true);
            Debug.Log($"Jugador activado: {jugadorActual.activeInHierarchy}");

            if (playerMovement != null)
            {
                playerMovement.FullResetMovement();
                playerMovement.enabled = true;
                playerMovement.SetCanMove(true);
                Debug.Log("PlayerMovement restaurado");
            }
        }
        else
        {
            Debug.LogError("No se encontró jugador con tag 'Player' en la escena");
        }
    }

    private void BuscarJugador()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugadorActual = player;
            playerMovement = player.GetComponent<PlayerMovement>();
            Debug.Log("Jugador encontrado y cacheado");
        }
    }

    public void OcultarJugador()
    {
        if (jugadorActual != null)
        {
            PlayerMovement movement = jugadorActual.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.SetCanMove(false);

            jugadorActual.SetActive(false);
            Debug.Log("Jugador ocultado");
        }
        else
        {
            BuscarJugador();
            OcultarJugador();
        }
    }

    public void MostrarJugador()
    {
        if (jugadorActual != null)
        {
            jugadorActual.SetActive(true);

            PlayerMovement movement = jugadorActual.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.SetCanMove(true);
                movement.enabled = true;
            }
            Debug.Log("Jugador mostrado");
        }
        else
        {
            BuscarJugador();
            MostrarJugador();
        }
    }

    public bool JugadorVisible()
    {
        return jugadorActual != null && jugadorActual.activeInHierarchy;
    }
}