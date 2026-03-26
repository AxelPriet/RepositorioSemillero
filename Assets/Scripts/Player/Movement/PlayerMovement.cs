using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float tileSize = 1f;

    private bool isMoving = false;
    private Vector2 moveInput;
    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private bool canMove = true;
    private bool isRunning = false;
    private float currentSpeed;
    private InputHandler inputHandler;
    private Animator animator;
    private static readonly int MovementHash = Animator.StringToHash("Movement");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        bool hayDialogo = DialogueManager.Instance != null && DialogueManager.Instance.IsActive;
        SetCanMove(!hayDialogo);

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
            StartCoroutine(MoveToTile(currentInput));

        UpdateAnimator(currentInput);
    }

    private void UpdateAnimator(Vector2 currentInput)
    {
        if (animator == null) return;

        float speed = 0f;
        if (currentInput != Vector2.zero)
            speed = isRunning ? 1f : 0.5f;

        animator.SetFloat(MovementHash, speed);
        animator.SetFloat(MoveXHash, currentInput.x);
        animator.SetFloat(MoveYHash, currentInput.y);

        if (currentInput.x > 0)
            spriteRenderer.flipX = true;
        else if (currentInput.x < 0)
            spriteRenderer.flipX = false;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat(MovementHash, 0f);
        animator.SetFloat(MoveXHash, 0f);
        animator.SetFloat(MoveYHash, 0f);
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
        {
            StopAllCoroutines();
            isMoving = false;
            UpdateAnimator();
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
            UpdateAnimator();
        }
    }

    public void ForceStopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        moveInput = Vector2.zero;
        UpdateAnimator();
    }

    public void FullResetMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        moveInput = Vector2.zero;
        targetPosition = transform.position;
        canMove = true;
        UpdateAnimator();
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
            UpdateAnimator();
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