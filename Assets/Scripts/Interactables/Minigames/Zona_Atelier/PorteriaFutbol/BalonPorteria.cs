using UnityEngine;

public class BalonPorteria : MonoBehaviour
{
    private Rigidbody2D rb;
    private float posicionPortero;
    private float porteroX;
    private float porteroY;
    private bool golMarcado = false;
    private bool atajado = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 0.5f;
        rb.gravityScale = 1f;
    }

    public void Lanzar(float angulo, float fuerza, float posPortero)
    {
        posicionPortero = posPortero;

        float anguloRad = angulo * Mathf.Deg2Rad;

        Vector2 direccion = new Vector2(
            -Mathf.Tan(anguloRad),
            Mathf.Cos(anguloRad)
        );

        rb.linearVelocity = direccion * fuerza * 3f;
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (golMarcado || atajado) return;

        if (other.gameObject.name == "Portero")
        {
            atajado = true;
            Debug.Log("¡Atajado!");
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.name == "ZonaIzquierda")
        {
            if (posicionPortero < -100f)
            {
                Debug.Log("Atajado a la izquierda");
                Destroy(gameObject);
            }
            else
            {
                golMarcado = true;
                FindFirstObjectByType<MiniGamePorteria>()?.RegistrarGol("izquierda");
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.name == "ZonaCentro")
        {
            if (Mathf.Abs(posicionPortero) > 100f)
            {
                Debug.Log("Atajado al centro");
                Destroy(gameObject);
            }
            else
            {
                golMarcado = true;
                FindFirstObjectByType<MiniGamePorteria>()?.RegistrarGol("centro");
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.name == "ZonaDerecha")
        {
            if (posicionPortero > 100f)
            {
                Debug.Log("Atajado a la derecha");
                Destroy(gameObject);
            }
            else
            {
                golMarcado = true;
                FindFirstObjectByType<MiniGamePorteria>()?.RegistrarGol("derecha");
                Destroy(gameObject);
            }
        }
    }
}