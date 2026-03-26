using UnityEngine;

public class PersistentGuide : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false); 
    }
}
