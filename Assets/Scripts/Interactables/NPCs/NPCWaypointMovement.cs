using UnityEngine;
using System.Collections;

public class NPCWaypointMovement : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private bool loop = true;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moviendoBool = "Moviendo";
    [SerializeField] private string direccionInt = "Direccion";

    private int currentWaypoint = 0;
    private bool isWaiting = false;
    private bool isMoving = true;
    private bool isStopped = false;
    private Rigidbody2D rb;
    private Vector2 lastDirection = Vector2.down; 

    public bool IsMoving => isMoving && !isStopped;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, false);
            if (!string.IsNullOrEmpty(direccionInt))
                animator.SetInteger(direccionInt, 0); 
        }

        if (waypoints.Length > 0)
            StartCoroutine(MoveToNextWaypoint());
    }

    private IEnumerator MoveToNextWaypoint()
    {
        while (true)
        {
            if (isStopped || waypoints.Length == 0)
            {
                yield return null;
                continue;
            }

            if (isWaiting)
            {
                yield return new WaitForSeconds(waitTime);
                isWaiting = false;
            }

            Vector2 target = waypoints[currentWaypoint].position;
            isMoving = true;

            while (Vector2.Distance(transform.position, target) > 0.1f)
            {
                if (isStopped) break;

                Vector2 currentPos = transform.position;
                Vector2 direccionMovimiento = (target - currentPos).normalized;

                if (direccionMovimiento != Vector2.zero)
                    lastDirection = direccionMovimiento;

                Vector2 newPos = Vector2.MoveTowards(
                    currentPos,
                    target,
                    moveSpeed * Time.deltaTime
                );
                rb.MovePosition(newPos);

                ActualizarAnimacion(true, direccionMovimiento);

                yield return null;
            }

            isMoving = false;
            ActualizarAnimacion(false, lastDirection);

            if (currentWaypoint < waypoints.Length - 1)
                currentWaypoint++;
            else if (loop)
                currentWaypoint = 0;
            else
                yield break;

            isWaiting = true;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void ActualizarAnimacion(bool moviendo, Vector2 direccion)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(moviendoBool))
            animator.SetBool(moviendoBool, moviendo);

        if (moviendo && direccion != Vector2.zero)
        {
            ActualizarDireccion(direccion);
        }
        else if (!moviendo && lastDirection != Vector2.zero)
        {
           
        }
    }

    private void ActualizarDireccion(Vector2 dir)
    {
        if (animator == null || string.IsNullOrEmpty(direccionInt)) return;

        int direccion = 0;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Debug.Log($"Dirección recibida: {dir}");

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            direccion = 2;

            if (sr != null)
            {
                sr.flipX = dir.x > 0;
                Debug.Log($"flipX = {sr.flipX} (dir.x = {dir.x})");
            }
        }
        else
        {
            if (sr != null)
                sr.flipX = false;

            if (dir.y > 0)
                direccion = 1;
            else
                direccion = 0;
        }

        animator.SetInteger(direccionInt, direccion);
    }

    public void StopMovement()
    {
        isStopped = true;
        isMoving = false;
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, false);
        }
    }

    public void ResumeMovement()
    {
        isStopped = false;
        isMoving = true;
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(moviendoBool))
                animator.SetBool(moviendoBool, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        if (loop && waypoints.Length > 1)
        {
            if (waypoints[waypoints.Length - 1] != null && waypoints[0] != null)
                Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}