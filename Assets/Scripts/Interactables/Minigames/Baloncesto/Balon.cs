using UnityEngine;

public class Balon : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 0.5f;
        rb.gravityScale = 1f;
    }

    public void Lanzar(float angulo, float fuerza)
    {
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
        if (other.gameObject.name == "Aro")
        {
            FindFirstObjectByType<MinijuegoBaloncesto>()?.RegistrarCanasta();
            Destroy(gameObject);
        }
    }
}