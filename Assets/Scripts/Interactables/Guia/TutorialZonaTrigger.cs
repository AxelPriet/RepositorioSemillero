using UnityEngine;

public class TutorialZonaTrigger : MonoBehaviour
{
    [SerializeField] private string nombreZona; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        TutorialGuide guide = FindFirstObjectByType<TutorialGuide>();
        guide?.EntrarEnZona(nombreZona);
    }
}
