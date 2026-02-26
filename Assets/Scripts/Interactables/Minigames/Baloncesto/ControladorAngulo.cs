using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorAngulo : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform puntoLanzamiento;
    [SerializeField] private float anguloMinimo = 30f;
    [SerializeField] private float anguloMaximo = 150f;

    private float anguloActual = 45f;

    public float ObtenerAngulo() => anguloActual;

    public void Actualizar()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direccion = mousePos - (Vector2)puntoLanzamiento.position;
        anguloActual = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        anguloActual = Mathf.Clamp(anguloActual, anguloMinimo, anguloMaximo);
    }

    public void Resetear()
    {
        anguloActual = 45f;
    }
}
