using UnityEngine;

public class Balon : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Balón sin Rigidbody2D");
    }

    public void Lanzar(float angulo, float fuerza)
    {
        transform.SetParent(null);

        // Activar física
        rb.simulated = true;
        rb.mass = 0.3f; 
        rb.gravityScale = 1f;
        rb.linearDamping = 0f; 
        rb.angularDamping = 0f;

        // Calcular dirección
        float anguloRad = angulo * Mathf.Deg2Rad;
        Vector2 direccion = new Vector2(
            -Mathf.Cos(anguloRad),
            Mathf.Sin(anguloRad)
        );

        // APLICAR VELOCIDAD (aumentada para que se note)
        rb.linearVelocity = direccion * fuerza * 5f;

        Debug.Log($"Balón lanzado - Velocidad: {rb.linearVelocity}"); // ← VERIFICAR

        // Destruir después de 3 segundos
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Aro"))
        {
            FindFirstObjectByType<MinijuegoBaloncesto>()?.RegistrarCanasta();
            Destroy(gameObject);
        }
    }
}