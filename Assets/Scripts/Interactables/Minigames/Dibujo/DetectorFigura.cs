using UnityEngine;
using System.Collections.Generic;

public class DetectorFigura : MonoBehaviour
{
    public enum TipoFigura { Circulo, Cuadrado, Triangulo }

    [Header("Configuración")]
    [SerializeField] private TipoFigura figura;
    [SerializeField] private Vector2 centro;
    [SerializeField] private float tamaño;
    [SerializeField] private float grosorBorde = 10f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnDrawGizmos()
    {
        Vector3 worldPos = transform.position + new Vector3(centro.x, centro.y, 0);

        switch (figura)
        {
            case TipoFigura.Circulo:
                Gizmos.color = new Color(1, 0, 0, 0.2f);
                Gizmos.DrawSphere(worldPos, tamaño);
                Gizmos.color = new Color(0, 1, 0, 0.2f);
                Gizmos.DrawSphere(worldPos, tamaño - grosorBorde);
                break;

            case TipoFigura.Cuadrado:
                Gizmos.color = new Color(1, 0, 0, 0.2f);
                Gizmos.DrawCube(worldPos, new Vector3(tamaño, tamaño, 0));
                Gizmos.color = new Color(0, 1, 0, 0.2f);
                Gizmos.DrawCube(worldPos, new Vector3(tamaño - grosorBorde * 2, tamaño - grosorBorde * 2, 0));
                break;

            case TipoFigura.Triangulo:
                float altura = tamaño * Mathf.Sqrt(3) / 2f;

                Vector3 v1 = worldPos + new Vector3(-tamaño / 2, -altura / 3, 0);
                Vector3 v2 = worldPos + new Vector3(tamaño / 2, -altura / 3, 0);
                Vector3 v3 = worldPos + new Vector3(0, altura * 2 / 3, 0);

                // Triángulo completo (rojo)
                Gizmos.color = new Color(1, 0, 0, 0.2f);
                DibujarTrianguloGizmos(v1, v2, v3);

                // Triángulo interior (verde)
                float escala = (tamaño - grosorBorde) / tamaño;
                Vector3 v1s = worldPos + new Vector3(-tamaño / 2 * escala, -altura / 3 * escala, 0);
                Vector3 v2s = worldPos + new Vector3(tamaño / 2 * escala, -altura / 3 * escala, 0);
                Vector3 v3s = worldPos + new Vector3(0, altura * 2 / 3 * escala, 0);

                Gizmos.color = new Color(0, 1, 0, 0.2f);
                DibujarTrianguloGizmos(v1s, v2s, v3s);
                break;
        }
    }

    private void DibujarTrianguloGizmos(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v1);
    }

    public bool EstaEnZonaSegura(Vector2 punto)
    {
        // Convertir punto a coordenadas locales del detector
        Vector2 puntoLocal = transform.InverseTransformPoint(punto);
        puntoLocal -= centro;

        switch (figura)
        {
            case TipoFigura.Circulo:
                return puntoLocal.magnitude < tamaño - grosorBorde;
            case TipoFigura.Cuadrado:
                return Mathf.Abs(puntoLocal.x) < tamaño / 2 - grosorBorde &&
                       Mathf.Abs(puntoLocal.y) < tamaño / 2 - grosorBorde;
            case TipoFigura.Triangulo:
                float altura = tamaño * Mathf.Sqrt(3) / 2f;
                Vector2 v1 = new Vector2(-tamaño / 2, -altura / 3);
                Vector2 v2 = new Vector2(tamaño / 2, -altura / 3);
                Vector2 v3 = new Vector2(0, altura * 2 / 3);
                return PuntoEnTriangulo(puntoLocal, v1, v2, v3);
            default:
                return false;
        }
    }

    public bool EstaEnBorde(Vector2 punto)
    {
        Vector2 puntoLocal = transform.InverseTransformPoint(punto);
        puntoLocal -= centro;

        switch (figura)
        {
            case TipoFigura.Circulo:
                float dist = puntoLocal.magnitude;
                return dist > tamaño - grosorBorde && dist < tamaño;
            case TipoFigura.Cuadrado:
                bool dentroX = Mathf.Abs(puntoLocal.x) < tamaño / 2;
                bool dentroY = Mathf.Abs(puntoLocal.y) < tamaño / 2;
                bool cercaBordeX = Mathf.Abs(puntoLocal.x) > tamaño / 2 - grosorBorde;
                bool cercaBordeY = Mathf.Abs(puntoLocal.y) > tamaño / 2 - grosorBorde;
                return dentroX && dentroY && (cercaBordeX || cercaBordeY);
            case TipoFigura.Triangulo:
                float altura = tamaño * Mathf.Sqrt(3) / 2f;
                Vector2 v1 = new Vector2(-tamaño / 2, -altura / 3);
                Vector2 v2 = new Vector2(tamaño / 2, -altura / 3);
                Vector2 v3 = new Vector2(0, altura * 2 / 3);

                if (!PuntoEnTriangulo(puntoLocal, v1, v2, v3)) return false;

                float d1 = DistanciaPuntoSegmento(puntoLocal, v1, v2);
                float d2 = DistanciaPuntoSegmento(puntoLocal, v2, v3);
                float d3 = DistanciaPuntoSegmento(puntoLocal, v3, v1);

                return Mathf.Min(d1, d2, d3) < grosorBorde;
            default:
                return false;
        }
    }

    private bool EstaEnCirculo(Vector2 punto, bool borde)
    {
        float distancia = Vector2.Distance(punto, centro);

        if (borde)
        {
            return distancia > tamaño - grosorBorde && distancia < tamaño + grosorBorde;
        }
        else
        {
            return distancia < tamaño - grosorBorde;
        }
    }

    private bool EstaEnCuadrado(Vector2 punto, bool borde)
    {
        float minX = centro.x - tamaño / 2;
        float maxX = centro.x + tamaño / 2;
        float minY = centro.y - tamaño / 2;
        float maxY = centro.y + tamaño / 2;

        if (borde)
        {
            bool cercaBordeX = Mathf.Abs(punto.x - minX) < grosorBorde ||
                               Mathf.Abs(punto.x - maxX) < grosorBorde;
            bool cercaBordeY = Mathf.Abs(punto.y - minY) < grosorBorde ||
                               Mathf.Abs(punto.y - maxY) < grosorBorde;

            return (punto.x > minX && punto.x < maxX &&
                    punto.y > minY && punto.y < maxY) &&
                    (cercaBordeX || cercaBordeY);
        }
        else
        {
            return punto.x > minX + grosorBorde && punto.x < maxX - grosorBorde &&
                   punto.y > minY + grosorBorde && punto.y < maxY - grosorBorde;
        }
    }

    private bool EstaEnTriangulo(Vector2 punto, bool borde)
    {
        float altura = tamaño * Mathf.Sqrt(3) / 2f;

        Vector2 v1 = new Vector2(centro.x - tamaño / 2, centro.y - altura / 3);
        Vector2 v2 = new Vector2(centro.x + tamaño / 2, centro.y - altura / 3);
        Vector2 v3 = new Vector2(centro.x, centro.y + altura * 2 / 3);

        bool dentro = PuntoEnTriangulo(punto, v1, v2, v3);

        if (borde && dentro)
        {
            return DistanciaAlTriangulo(punto, v1, v2, v3) < grosorBorde;
        }
        else
        {
            return dentro;
        }
    }

    private bool PuntoEnTriangulo(Vector2 p, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float areaTotal = Mathf.Abs((v2.x - v1.x) * (v3.y - v1.y) - (v3.x - v1.x) * (v2.y - v1.y)) / 2f;

        float area1 = Mathf.Abs((v1.x - p.x) * (v2.y - p.y) - (v2.x - p.x) * (v1.y - p.y)) / 2f;
        float area2 = Mathf.Abs((v2.x - p.x) * (v3.y - p.y) - (v3.x - p.x) * (v2.y - p.y)) / 2f;
        float area3 = Mathf.Abs((v3.x - p.x) * (v1.y - p.y) - (v1.x - p.x) * (v3.y - p.y)) / 2f;

        return Mathf.Approximately(area1 + area2 + area3, areaTotal);
    }

    private float DistanciaAlTriangulo(Vector2 p, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1 = DistanciaPuntoSegmento(p, v1, v2);
        float d2 = DistanciaPuntoSegmento(p, v2, v3);
        float d3 = DistanciaPuntoSegmento(p, v3, v1);

        return Mathf.Min(d1, d2, d3);
    }

    private float DistanciaPuntoSegmento(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;

        float t = Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab);
        t = Mathf.Clamp01(t);

        Vector2 puntoCercano = a + t * ab;
        return Vector2.Distance(p, puntoCercano);
    }
}