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
        if (scene.buildIndex == 0) 
        {
            if (jugadorActual != null)
            {
                OcultarJugador();
            }
            else
            {
                BuscarJugador();
                if (jugadorActual != null)
                    OcultarJugador();
            }
        }
        else if (scene.buildIndex == 1)
        {
            StartCoroutine(ReactivarJugadorProximoFrame(scene));
        }
    }

    private IEnumerator ReactivarJugadorProximoFrame(Scene scene)
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugadorActual = player;
            playerMovement = player.GetComponent<PlayerMovement>();

            jugadorActual.SetActive(true);

            if (playerMovement != null)
            {
                playerMovement.FullResetMovement();
                playerMovement.enabled = true;
            }
        }
    }

    private void BuscarJugador()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugadorActual = player;
            playerMovement = player.GetComponent<PlayerMovement>();
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