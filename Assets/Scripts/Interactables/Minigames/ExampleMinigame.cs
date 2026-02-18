using UnityEngine;

public class ExampleMinigame : MonoBehaviour, IInteractuable, IMinigame
{
    [SerializeField] private int minigameIndex;

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
            return "Este minijuego aún no está disponible";

        return "Presiona E para iniciar";
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
        Debug.Log($"Iniciando minijuego {minigameIndex}");

        Invoke(nameof(FinishTest), 2f);
    }

    private void FinishTest()
    {
        CompleteMinigame();
    }

    public void CompleteMinigame()
    {
        Debug.Log($"Minijuego {minigameIndex} completado");

        MinigameManager.Instance.CompleteMinigame();
    }

    public void FailMinigame()
    {
        Debug.Log("Minijuego fallado");

        MinigameManager.Instance.FailMinigame();
    }
}
