using UnityEngine;

public class ControladorBalon : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject balonPrefab;
    [SerializeField] private Transform puntoLanzamiento;

    private GameObject balonActual;
    private Rigidbody2D rbBalon;

    public void Lanzar(float angulo, float fuerza)
    {
        balonActual = Instantiate(balonPrefab, puntoLanzamiento.position, Quaternion.identity);
        rbBalon = balonActual.GetComponent<Rigidbody2D>();

        if (rbBalon == null)
        {
            Debug.LogError("El prefab del balón no tiene Rigidbody2D");
            return;
        }

        Vector2 velocidad = new Vector2(
            Mathf.Cos(angulo * Mathf.Deg2Rad) * fuerza,
            Mathf.Sin(angulo * Mathf.Deg2Rad) * fuerza
        );

        rbBalon.linearVelocity = velocidad;
    }
}
