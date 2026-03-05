using UnityEngine;

public class ZonaGol : MonoBehaviour
{
    private void Awake()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }
}