using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameCombinado : MonoBehaviour, IMinigame, IInteractuable
{
    [Header("Configuración del Minijuego Combinado")]
    [SerializeField] private int minigameIndex = 1; 

    [Header("Sub-Minijuegos")]
    [SerializeField] private GameObject[] subMinijuegosUI; 
    private ISubMinigame currentSubMinijuego;
    private int currentSubIndex = 0;

    private bool minijuegoIniciado = false;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = InputHandler.Instance.GetControls();
    }

    // IInteractuable
    public void Interactuar()
    {
        if (!GameProgressManager.Instance.CanPlayMinigame(minigameIndex))
        {
            Debug.Log("Debes completar el minijuego anterior primero.");
            return;
        }

        MinigameManager.Instance.StartMinigame(this, minigameIndex);
    }

    public string GetPrompt()
    {
        if (minijuegoIniciado) return ""; 
        return "Iniciar Minijuego Combinado";
    }

    public bool PuedeInteractuar()
    {
        return !minijuegoIniciado && GameProgressManager.Instance.CanPlayMinigame(minigameIndex);
    }

    public Transform GetTransform() => transform;

    // IMinigame
    public void StartMinigame()
    {
        minijuegoIniciado = true;
        currentSubIndex = 0;
        StartNextSubMinijuego();
    }

    public void CompleteMinigame()
    {
        GameProgressManager.Instance.CompleteMinigame();
        CloseAllSubMinijuegos();
    }

    public void FailMinigame()
    {
        Debug.Log("Minijuego fallido. Debes intentarlo de nuevo.");
        CloseAllSubMinijuegos();
    }

    private void StartNextSubMinijuego()
    {
        if (currentSubIndex >= subMinijuegosUI.Length)
        {
            CompleteMinigame();
            return;
        }

        foreach (var ui in subMinijuegosUI)
            ui.SetActive(false);

        subMinijuegosUI[currentSubIndex].SetActive(true);
        currentSubMinijuego = subMinijuegosUI[currentSubIndex].GetComponent<ISubMinigame>();
        if (currentSubMinijuego != null)
            currentSubMinijuego.StartSubMinigame(this);
    }


    public void OnSubMinijuegoComplete()
    {
        currentSubIndex++;
        StartNextSubMinijuego();
    }

    public void OnSubMinijuegoFail()
    {
        currentSubIndex = 0;
        StartNextSubMinijuego();
    }

    private void CloseAllSubMinijuegos()
    {
        foreach (var ui in subMinijuegosUI)
            ui.SetActive(false);
    }
}
