using UnityEngine;
using System.Collections.Generic;

public class DetectorFigura : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform[] puntosReferencia; 
    [SerializeField] private float tolerancia = 30f; 

    private bool[] puntosCompletados;
    private int puntosCorrectos = 0;

    private void Awake()
    {
        puntosCompletados = new bool[puntosReferencia.Length];
    }

    public bool VerificarPunto(Vector2 puntoJugador)
    {
        for (int i = 0; i < puntosReferencia.Length; i++)
        {
            if (puntosCompletados[i]) continue;

            float distancia = Vector2.Distance(puntoJugador, puntosReferencia[i].position);

            if (distancia < tolerancia)
            {
                puntosCompletados[i] = true;
                puntosCorrectos++;
                return true;
            }
        }
        return false;
    }

    public float ObtenerProgreso()
    {
        return (float)puntosCorrectos / puntosReferencia.Length * 100f;
    }

    public bool EstaCompletada()
    {
        return puntosCorrectos >= puntosReferencia.Length;
    }

    private void OnDrawGizmos()
    {
        if (puntosReferencia == null) return;

        Gizmos.color = Color.green;
        foreach (Transform punto in puntosReferencia)
        {
            Gizmos.DrawWireSphere(punto.position, tolerancia);
        }
    }
}