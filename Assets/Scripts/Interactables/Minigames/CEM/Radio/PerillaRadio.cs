using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerillaRadio : MonoBehaviour, IDragHandler
{
    [Header("Configuración")]
    [SerializeField] private float sensibilidad = 0.5f;
    [SerializeField] private float anguloMinimo = -90f;
    [SerializeField] private float anguloMaximo = 90f;

    private float valorActual = 0.5f; // 0 a 1

    public float ValorNormalizado => valorActual;

    public void OnDrag(PointerEventData eventData)
    {
        // Rotar basado en movimiento horizontal del mouse
        float rotacion = -eventData.delta.x * sensibilidad;

        // Aplicar rotación
        transform.Rotate(0, 0, rotacion);

        // Limitar rotación
        Vector3 angulos = transform.localEulerAngles;
        if (angulos.z > 180) angulos.z -= 360;

        angulos.z = Mathf.Clamp(angulos.z, anguloMinimo, anguloMaximo);
        transform.localEulerAngles = angulos;

        // Calcular valor (0-1) basado en el ángulo
        valorActual = Mathf.InverseLerp(anguloMinimo, anguloMaximo, angulos.z);
    }

    public void ResetearPerilla()
    {
        transform.localEulerAngles = Vector3.zero;
        valorActual = 0.5f;
    }
}