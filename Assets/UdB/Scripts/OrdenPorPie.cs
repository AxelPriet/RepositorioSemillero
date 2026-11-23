using UnityEngine;

/// <summary>
/// Para objetos sueltos del mapa (autos estacionados, etc.) con el pivote en su base.
/// Si el personaje está más al sur que la base del objeto (delante), el objeto se dibuja debajo de él;
/// si está más al norte (detrás), el objeto se dibuja encima y lo tapa.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class OrdenPorPie : MonoBehaviour
{
    [Tooltip("Tag del personaje.")]
    public string tagJugador = "Player";
    [Tooltip("Distancia (en unidades) desde el pivote del personaje hasta sus pies.")]
    public float pies = 1f;
    [Tooltip("Cuánto más arriba de la base del objeto está la línea de 'detrás' (en unidades).")]
    public float margen = 0.5f;

    SpriteRenderer sr, srJugador;
    Transform jugador;

    void Start() { sr = GetComponent<SpriteRenderer>(); Buscar(); }

    void Buscar()
    {
        var go = GameObject.FindGameObjectWithTag(tagJugador);
        if (go == null) return;
        jugador = go.transform; srJugador = go.GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (jugador == null) { Buscar(); if (jugador == null) return; }
        bool detras = jugador.position.y - pies > transform.position.y + margen;
        int orden = (srJugador != null ? srJugador.sortingOrder : 0) + (detras ? 1 : -1);
        if (sr.sortingOrder != orden) sr.sortingOrder = orden;
    }
}
