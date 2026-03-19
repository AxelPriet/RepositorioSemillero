using UnityEngine;
using System.Collections.Generic;

public class GuiaManager : MonoBehaviour
{
    public static GuiaManager Instance;

    [Header("Configuración")]
    [SerializeField] private GuiaMensajes mensajes;
    [SerializeField] private GuiaUI guiaUI;
    [SerializeField] private int fallosConsecutivosParaAyuda = 3;

    private Dictionary<int, int> fallosPorMinijuego = new Dictionary<int, int>();
    private HashSet<int> minijuegosCompletados = new HashSet<int>();

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
            return;
        }

        if (guiaUI == null)
            guiaUI = FindFirstObjectByType<GuiaUI>();

        if (CarnetManager.Instance != null)
            CarnetManager.Instance.OnPieceCollected += OnPieceCollected;
    }

    private void OnDestroy()
    {
        if (CarnetManager.Instance != null)
            CarnetManager.Instance.OnPieceCollected -= OnPieceCollected;
    }

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
        {
            MostrarMensajeAleatorio(mensajes.bienvenida);
        }
    }


    public void OnMinigameEnter(int minigameIndex)
    {
        MostrarMensajeAleatorio(mensajes.entradaMinijuego);
    }

    public void OnMinigameSuccess(int minigameIndex)
    {
        if (!minijuegosCompletados.Contains(minigameIndex))
        {
            minijuegosCompletados.Add(minigameIndex);
            MostrarMensajeAleatorio(mensajes.exitoMinijuego);
        }
        if (fallosPorMinijuego.ContainsKey(minigameIndex))
            fallosPorMinijuego.Remove(minigameIndex);
    }

    public void OnMinigameFail(int minigameIndex)
    {
        if (fallosPorMinijuego.ContainsKey(minigameIndex))
            fallosPorMinijuego[minigameIndex]++;
        else
            fallosPorMinijuego[minigameIndex] = 1;

        int fallos = fallosPorMinijuego[minigameIndex];

        if (fallos >= fallosConsecutivosParaAyuda)
        {
            MostrarMensajeAleatorio(mensajes.consejosAvanzados);
        }
        else
        {
            MostrarMensajeAleatorio(mensajes.falloMinijuego);
        }
    }

    private void OnPieceCollected()
    {
        MostrarMensajeAleatorio(mensajes.progreso);
    }

    private void MostrarMensajeAleatorio(string[] array)
    {
        if (array == null || array.Length == 0) return;
        string mensaje = array[Random.Range(0, array.Length)];
        guiaUI.MostrarMensaje(mensaje);
    }

    public void MostrarMensaje(string mensaje, float duracion = -1)
    {
        guiaUI.MostrarMensaje(mensaje, duracion);
    }
}