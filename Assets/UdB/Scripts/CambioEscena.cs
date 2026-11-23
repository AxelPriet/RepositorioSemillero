using UnityEngine;

// Zona (trigger) que lleva al jugador a otra escena con fundido: entrar o salir de un edificio. En la escena destino
// aparece en el PuntoLlegada cuyo id es "punto".
[RequireComponent(typeof(Collider2D))]
public class CambioEscena : MonoBehaviour
{
    [Tooltip("Nombre de la escena (debe estar en Build Settings)")]
    public string escena;
    [Tooltip("Id del PuntoLlegada donde aparece el jugador en esa escena")]
    public string punto;

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (Transicion.EnCurso || !otro.CompareTag("Player")) return;
        Transicion.IrA(escena, punto);
    }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = new Color(1f, 0.55f, 0.1f, 0.4f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
