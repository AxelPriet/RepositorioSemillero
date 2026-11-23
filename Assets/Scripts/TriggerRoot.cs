using UnityEngine;

public class EdgeTrigger : MonoBehaviour
{
    public WorldRotator worldRotator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            worldRotator.ToggleRotation();
        }
    }
}