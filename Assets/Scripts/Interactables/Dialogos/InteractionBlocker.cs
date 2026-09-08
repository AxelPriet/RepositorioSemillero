using UnityEngine;

public class InteractionBlocker : MonoBehaviour
{
    public static InteractionBlocker Instance { get; private set; }

    private bool isBlocked = false;

    public bool IsBlocked => isBlocked;

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

    public void BlockInteractions()
    {
        isBlocked = true;
        Debug.Log("Interacciones BLOQUEADAS");
    }

    public void UnblockInteractions()
    {
        isBlocked = false;
        Debug.Log("Interacciones DESBLOQUEADAS");
    }
}