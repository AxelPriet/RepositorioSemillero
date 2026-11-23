using UnityEngine;

public class TilemapFollowPlayer : MonoBehaviour
{
    public Transform target;
    public float tileSize = 1f;
    public bool followX = true;
    public bool followY = true;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;

        if (followX)
            pos.x = Mathf.Floor(target.position.x / tileSize) * tileSize;

        if (followY)
            pos.y = Mathf.Floor(target.position.y / tileSize) * tileSize;

        transform.position = pos;
    }
}