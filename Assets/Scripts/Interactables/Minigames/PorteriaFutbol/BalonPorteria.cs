using UnityEngine;

public class BalonPorteria : MonoBehaviour
{
    private Rigidbody2D rb;
    private float posicionPortero;
    private float porteroX;
    private float porteroY;
    private bool golMarcado = false;

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
            Mathf.Tan(anguloRad),
            Mathf.Cos(anguloRad)
        );

        rb.linearVelocity = direccion * fuerza * 3f;
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (golMarcado) return;

        string zona = other.tag; 

        bool porteroEnZona = false;

        if (zona == "Izquierda" && porteroX < -100f)
            porteroEnZona = true;
        else if (zona == "Centro" && Mathf.Abs(porteroX) < 100f)
            porteroEnZona = true;
        else if (zona == "Derecha" && porteroX > 100f)
            porteroEnZona = true;

        // Si el portero NO está en la zona, es gol
        if (!porteroEnZona)
        {
            golMarcado = true;
            FindFirstObjectByType<MiniGamePorteria>()?.RegistrarGol(zona);
        }

        Destroy(gameObject, 0.1f);
    }
}