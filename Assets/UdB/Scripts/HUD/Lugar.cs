using System.Collections.Generic;
using UnityEngine;

// Zona con nombre del campus (pedido 2026-11-17): cuando el jugador entra, la Cartelera muestra "Estás en <nombre>".
// Si el jugador está dentro de varias zonas a la vez (una plaza dentro de un sector), gana la de mayor prioridad y,
// a igual prioridad, la más pequeña. Un mismo lugar no se vuelve a anunciar hasta pasar por otro.
// Va en un objeto con un Collider2D (Box o Polygon) marcado como Is Trigger.
[RequireComponent(typeof(Collider2D))]
public class Lugar : MonoBehaviour
{
    [Tooltip("Nombre que sale en la cartelera y bajo el minimapa")]
    public string nombre = "Lugar";
    [Tooltip("Gana sobre otras zonas que se cruzan con esta (mayor = más importante)")]
    public int prioridad = 0;

    /// <summary>Lugar donde está ahora el jugador (null si en ninguno).</summary>
    public static Lugar Actual { get; private set; }
    static readonly List<Lugar> dentro = new List<Lugar>();
    static string ultimoAnunciado;

    float Area { get { var b = GetComponent<Collider2D>().bounds; return b.size.x * b.size.y; } }

    void Reset() { GetComponent<Collider2D>().isTrigger = true; nombre = gameObject.name; }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player") || dentro.Contains(this)) return;
        dentro.Add(this); Elegir();
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;
        if (dentro.Remove(this)) Elegir();
    }

    void OnDisable() { if (dentro.Remove(this)) Elegir(); }

    static void Elegir()
    {
        dentro.RemoveAll(l => l == null);
        Lugar mejor = null;
        foreach (var l in dentro)
            if (mejor == null || l.prioridad > mejor.prioridad || (l.prioridad == mejor.prioridad && l.Area < mejor.Area)) mejor = l;
        Actual = mejor;
        if (mejor != null && mejor.nombre != ultimoAnunciado)
        {
            ultimoAnunciado = mejor.nombre;
            Cartelera.Mostrar("Estás en", mejor.nombre);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Limpiar() { dentro.Clear(); Actual = null; ultimoAnunciado = null; }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.12f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.6f);
        Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(col.bounds.center, nombre);
#endif
    }
}
