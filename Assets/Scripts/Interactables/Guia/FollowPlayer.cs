using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(20f, 5f, 0f);
    [SerializeField] private float smoothSpeed = 5f;

    private void Start()
    {
        BuscarPlayer();
    }

    private void BuscarPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void LateUpdate()
    {
        if (player == null)
            BuscarPlayer();

        if (player == null) return;

        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}