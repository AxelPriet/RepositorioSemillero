using UnityEngine;

public class GuideTrigger : MonoBehaviour
{
    [SerializeField] private GuideManager.GuideEvent evento;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GuideManager.Instance?.TriggerEvent(evento);
    }
}