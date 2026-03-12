using UnityEngine;
using UnityEngine.EventSystems;

public class PerillaRadio : MonoBehaviour, IDragHandler
{
    [Header("Configuración")]
    [SerializeField] private float sensibilidad = 0.5f;
    [SerializeField] private float anguloMinimo = -90f;
    [SerializeField] private float anguloMaximo = 90f;

    private float valorActual = 0.5f;
    private RectTransform rectTransform;

    public float ValorNormalizado => valorActual;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float rotacion = -eventData.delta.x * sensibilidad;
        rectTransform.Rotate(0, 0, rotacion);

        Vector3 angulos = rectTransform.localEulerAngles;
        if (angulos.z > 180) angulos.z -= 360;
        angulos.z = Mathf.Clamp(angulos.z, anguloMinimo, anguloMaximo);
        rectTransform.localEulerAngles = angulos;

        valorActual = Mathf.InverseLerp(anguloMinimo, anguloMaximo, angulos.z);
    }

    public void ResetearPerilla()
    {
        rectTransform.localEulerAngles = Vector3.zero;
        valorActual = 0.5f;
    }
}