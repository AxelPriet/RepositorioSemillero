using UnityEngine;

public class ControladorTrayectoria : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject puntoPrefab;
    [SerializeField] private Transform puntoLanzamiento;
    [SerializeField] private int cantidadPuntos = 12;
    [SerializeField] private float separacionPuntos = 0.2f;

    private GameObject[] puntosTrayectoria;

    public void CrearPuntos(Transform parent)
    {
        if (puntoPrefab == null) return;

        puntosTrayectoria = new GameObject[cantidadPuntos];
        for (int i = 0; i < cantidadPuntos; i++)
        {
            puntosTrayectoria[i] = Instantiate(puntoPrefab, parent);
            puntosTrayectoria[i].SetActive(false);
        }
    }

    public void ActualizarPuntos(float angulo, float fuerza)
    {
        if (puntosTrayectoria == null || puntoLanzamiento == null) return;

        Vector2 velocidadInicial = new Vector2(
            Mathf.Cos(angulo * Mathf.Deg2Rad) * fuerza,
            Mathf.Sin(angulo * Mathf.Deg2Rad) * fuerza
        );

        for (int i = 0; i < cantidadPuntos; i++)
        {
            float t = (i + 1) * separacionPuntos;
            float x = puntoLanzamiento.position.x + velocidadInicial.x * t;
            float y = puntoLanzamiento.position.y + velocidadInicial.y * t + (0.5f * Physics2D.gravity.y * t * t);

            puntosTrayectoria[i].transform.position = new Vector3(x, y, 0);
            puntosTrayectoria[i].SetActive(true);
        }
    }

    public void OcultarPuntos()
    {
        if (puntosTrayectoria == null) return;

        foreach (GameObject punto in puntosTrayectoria)
        {
            if (punto != null)
                punto.SetActive(false);
        }
    }
}
