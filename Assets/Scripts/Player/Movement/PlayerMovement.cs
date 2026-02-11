using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tileSize = 1f;

    [Header("Estado Actual")]
    [SerializeField] private bool isMoving = false;

    // Dirección que recibirá del InputHandler
    private Vector2 moveInput;
    private Vector2 targetPosition;

    // Componentes
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
        // 🚨 ELIMINA COMPLETAMENTE GetMoveInput() de aquí
        // Solo procesa movimiento si hay input y no está moviéndose
        if (!isMoving && moveInput != Vector2.zero)
        {
            StartCoroutine(MoveToTile(moveInput));
        }
    }

    // 📌 ESTE ES EL MÉTODO QUE USA EL INPUTHANDLER
    public void SetMoveDirection(Vector2 direction)
    {
        moveInput = direction;
    }

    // 🗑️ ELIMINA COMPLETAMENTE este método:
    // private void GetMoveInput() { ... }  ← BORRAR

    private IEnumerator MoveToTile(Vector2 direction)
    {
        isMoving = true;

        Vector2 startPosition = transform.position;
        targetPosition = startPosition + (direction * tileSize);

        // Detección de colisiones
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

        // Movimiento suave
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
