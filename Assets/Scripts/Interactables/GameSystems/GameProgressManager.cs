using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [SerializeField] private int currentMinigameIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanPlayMinigame(int index)
    {
        return index == currentMinigameIndex;
    }

    public void CompleteMinigame()
    {
        currentMinigameIndex++;
        CarnetManager.Instance.AddCarnetPiece();
    }

    public int GetCurrentIndex()
    {
        return currentMinigameIndex;
    }
}

