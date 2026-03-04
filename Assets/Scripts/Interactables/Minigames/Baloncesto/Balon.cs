using UnityEngine;

public class Balon : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 0.6f;
        rb.gravityScale = 1.2f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.3f;
    }

    public void Lanzar(float angulo, float fuerza)
    {
        float anguloRad = angulo * Mathf.Deg2Rad;

        Vector2 direccion = new Vector2(
            -Mathf.Abs(Mathf.Cos(anguloRad)),
            Mathf.Sin(anguloRad)
        );

        rb.linearVelocity = direccion * fuerza * 3f;
        rb.angularVelocity = Random.Range(-200f, 200f);

        Destroy(gameObject, 4f);
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