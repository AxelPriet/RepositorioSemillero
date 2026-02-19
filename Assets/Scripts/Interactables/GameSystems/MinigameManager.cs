using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [SerializeField] private GameObject[] minigamePanels;

    private IMinigame currentMinigame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; 
        }

        foreach (var panel in minigamePanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    public void StartMinigame(IMinigame minigame, int index)
    {
        currentMinigame = minigame;

        minigamePanels[index].SetActive(true);
        DisablePlayer();

        currentMinigame.StartMinigame();
    }


    public void CompleteMinigame()
    {
        GameProgressManager.Instance.CompleteMinigame();

        CloseMinigame();
    }

    public void FailMinigame()
    {
        CloseMinigame();
    }

    private void CloseMinigame()
    {
        foreach (var panel in minigamePanels)
            panel.SetActive(false);

        EnablePlayer();
    }

    private void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.SetCanMove(false);
    }

    private void EnablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.SetCanMove(true); 
    }

}