using UnityEngine;
using System.Collections.Generic;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [SerializeField] private int currentMinigameIndex = 0;
    private HashSet<int> completedMinigames = new HashSet<int>();

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

    public void CompleteMinigame(int minigameIndex)
    {
        if (completedMinigames.Contains(minigameIndex))
            return;

        completedMinigames.Add(minigameIndex);

        if (minigameIndex == currentMinigameIndex)
            currentMinigameIndex++;

        if (CarnetManager.Instance != null)
            CarnetManager.Instance.AddCarnetPiece();
    }

    public int GetCurrentIndex() => currentMinigameIndex;
}