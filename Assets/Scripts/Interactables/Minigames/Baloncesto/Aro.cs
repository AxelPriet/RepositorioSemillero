using UnityEngine;

public class Aro : MonoBehaviour
{
    private void Awake()
    {
        gameObject.tag = "Aro";
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }
}
