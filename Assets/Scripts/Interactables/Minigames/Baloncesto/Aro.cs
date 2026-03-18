using UnityEngine;

public class Aro : MonoBehaviour
{
    private void Awake()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        gameObject.name = "Aro";
    }
}