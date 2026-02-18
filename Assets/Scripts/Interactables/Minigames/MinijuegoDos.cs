using UnityEngine;
using EclipseGames.Player.Interaction;

public class MinijuegoDos : MonoBehaviour, IInteractuable, IMinigame
{
    [SerializeField] private int minigameIndex = 1;

    public bool PuedeInteractuar()
    {
        return GameProgressManager.Instance.CanPlayMinigame(minigameIndex);
    }

    public void Interactuar()
    {
        MinigameManager.Instance.StartMinigame(this, minigameIndex);
    }

    public string GetPrompt()
    {
        if (!GameProgressManager.Instance.CanPlayMinigame(minigameIndex))
            return "Debes completar el Minijuego 1 primero";

        return "Presiona E para iniciar Minijuego 2";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // =========================
    // IMinigame
    // =========================

    public void StartMinigame()
    {
        Debug.Log("Minijuego 2 iniciado");

        Invoke(nameof(FinishTest), 3f);
    }

    private void FinishTest()
    {
        CompleteMinigame();
    }

    public void CompleteMinigame()
    {
        Debug.Log("Minijuego 2 completado");

        MinigameManager.Instance.CompleteMinigame();
    }

    public void FailMinigame()
    {
        MinigameManager.Instance.FailMinigame();
    }
}
