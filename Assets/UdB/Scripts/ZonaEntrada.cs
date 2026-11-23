using UnityEngine;

// Zona (trigger) frente a la entrada de un edificio: mientras el jugador está dentro se dibuja DETRÁS de los edificios
// (mismo orden que los Muros, que están un poco más cerca de la cámara: z -0.05), así el techo o la fachada lo tapan y
// parece que entra. Al salir de la zona vuelve a su orden normal. (No alejar al jugador en Z: el fondo escribe
// profundidad y lo taparía entero.)
[RequireComponent(typeof(Collider2D))]
public class ZonaEntrada : MonoBehaviour
{
    [Tooltip("Orden de dibujo del jugador dentro de la zona (los Muros de los edificios están en -2)")]
    public int ordenDentro = -2;
    [Tooltip("Corrimiento en Z del jugador dentro de la zona (normalmente 0; ver nota arriba)")]
    public float zDentro = 0f;

    int dentro;
    SpriteRenderer sr;
    int ordenOriginal;
    float zOriginal;

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;
        if (dentro++ > 0) return;
        sr = otro.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;
        ordenOriginal = sr.sortingOrder; zOriginal = sr.transform.localPosition.z;
        sr.sortingOrder = ordenDentro;
        var p = sr.transform.localPosition; p.z = zOriginal + zDentro; sr.transform.localPosition = p;
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;
        if (--dentro > 0) return;
        dentro = 0;
        Restaurar();
    }

    void OnDisable() { if (dentro > 0) { dentro = 0; Restaurar(); } }

    void Restaurar()
    {
        if (sr == null) return;
        sr.sortingOrder = ordenOriginal;
        var p = sr.transform.localPosition; p.z = zOriginal; sr.transform.localPosition = p;
        sr = null;
    }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = new Color(0.6f, 0.3f, 1f, 0.3f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
