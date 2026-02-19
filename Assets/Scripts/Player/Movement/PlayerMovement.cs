using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tileSize = 1f;

    [Header("Estado Actual")]
    [SerializeField] private bool isMoving = false;
    private Vector2 moveInput;
    private Vector2 targetPosition;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving && moveInput != Vector2.zero)
        {
            StartCoroutine(MoveToTile(moveInput));
        }
    }
    public void ForceStopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        moveInput = Vector2.zero;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveInput = direction;
    }

    private IEnumerator MoveToTile(Vector2 direction)
    {
        isMoving = true;

        Vector2 startPosition = transform.position;
        targetPosition = startPosition + (direction * tileSize);

        float distance = Vector2.Distance(startPosition, targetPosition);
        RaycastHit2D hit = Physics2D.BoxCast(
            targetPosition,
            boxCollider.bounds.size,
            0f,
            Vector2.zero,
            distance,
            LayerMask.GetMask("Obstacle")
        );

        if (hit.collider != null)
        {
            isMoving = false;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
