using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float tileSize = 1f;

    [Header("Estado Actual")]
    [SerializeField] private bool isMoving = false;
    private Vector2 moveInput;
    private Vector2 targetPosition;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private bool canMove = true;
    private bool isRunning = false;
    private float currentSpeed;
    private InputHandler inputHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        targetPosition = transform.position;
        currentSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        StartCoroutine(InicializarMovimiento());
    }

    private IEnumerator InicializarMovimiento()
    {
        yield return new WaitForSeconds(0.1f);


        FullResetMovement();
        SetCanMove(true);

        inputHandler = InputHandler.Instance;

    }

    private void Update()
    {
        if (!canMove) return;

        if (inputHandler == null)
        {
            inputHandler = InputHandler.Instance;
            if (inputHandler == null) return;
        }

        isRunning = inputHandler.IsRunning();
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector2 currentInput = inputHandler.GetMoveInput();

        if (!isMoving && currentInput != Vector2.zero)
        {
            StartCoroutine(MoveToTile(currentInput));
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            moveInput = Vector2.zero;
            StopAllCoroutines();
            isMoving = false;
        }
    }

    public void ForceStopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        moveInput = Vector2.zero;
    }

    public void FullResetMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        moveInput = Vector2.zero;
        targetPosition = transform.position;
        canMove = true; 
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
            elapsedTime += Time.deltaTime * currentSpeed;
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}