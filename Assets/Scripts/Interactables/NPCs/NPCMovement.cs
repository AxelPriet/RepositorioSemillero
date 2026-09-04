using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Patrullaje")]
    [SerializeField] public bool patrolEnabled = false;
    [SerializeField] public Transform patrolCenter;
    [SerializeField] public float patrolRadius = 5f;
    [SerializeField] public float patrolSpeed = 2f;
    [SerializeField] private Color colorPatrullaje = Color.green;

    public event System.Action<Vector2> OnDirectionChanged;
    public Vector2 CurrentDirection { get; private set; } = Vector2.zero;

    private Vector2 patrolDirection;
    private Vector2 patrolCenterPos;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (patrolEnabled && patrolRadius > 0f)
        {
            patrolCenterPos = patrolCenter != null
                ? (Vector2)patrolCenter.position
                : (Vector2)transform.position;
            patrolDirection = Random.insideUnitCircle.normalized;
        }
    }

    private void Update()
    {
        if (patrolEnabled && patrolRadius > 0f)
            PatrolUpdate();
        else
            SetDirection(Vector2.zero);
    }

    private void PatrolUpdate()
    {
        Vector2 currentPos = transform.position;
        Vector2 nextPos = currentPos + patrolDirection * patrolSpeed * Time.deltaTime;

        // Detección de obstáculos
        RaycastHit2D hit = Physics2D.Raycast(
            currentPos,
            patrolDirection,
            patrolSpeed * Time.deltaTime,
            LayerMask.GetMask("Obstacle")
        );

        if (hit.collider != null)
        {
            patrolDirection = Vector2.Reflect(patrolDirection, hit.normal).normalized;
            return;
        }

        // Límite del radio
        if (Vector2.Distance(nextPos, patrolCenterPos) > patrolRadius)
        {
            Vector2 dirAlCentro = (patrolCenterPos - currentPos).normalized;
            float angulo = Random.Range(-45f, 45f);
            patrolDirection = RotarVector(dirAlCentro, angulo);
            return;
        }

        // Movimiento
        if (rb != null)
            rb.MovePosition(nextPos);
        else
            transform.position = nextPos;

        SetDirection(patrolDirection);
    }

    private Vector2 RotarVector(Vector2 v, float grados)
    {
        float rad = grados * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y).normalized;
    }

    private void SetDirection(Vector2 dir)
    {
        if (dir != CurrentDirection)
        {
            CurrentDirection = dir;
            OnDirectionChanged?.Invoke(dir);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolEnabled && patrolRadius > 0f)
        {
            Vector3 centerPos = patrolCenter != null
                ? patrolCenter.position
                : transform.position;
            Gizmos.color = colorPatrullaje;
            Gizmos.DrawWireSphere(centerPos, patrolRadius);
        }
    }
}