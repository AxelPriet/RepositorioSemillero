using UnityEngine;

// Aviso antes de una entrada (pedido 2026-11-17): al pisar el trigger, la Cartelera dice por ejemplo "Entrada al E2".
// Va en un objeto con un Collider2D marcado como Is Trigger, un poco antes de la puerta (junto a su FlechaEntrada).
[RequireComponent(typeof(Collider2D))]
public class AvisoEntrada : MonoBehaviour
{
    [Tooltip("Texto grande de la cartelera")]
    public string texto = "Entrada";
    [Tooltip("Texto pequeño de arriba")]
    public string titulo = "Entrada";
    [Tooltip("Segundos antes de poder volver a avisar")]
    public float espera = 4f;

    float ultimo = -999f;

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player") || Time.unscaledTime - ultimo < espera) return;
        if (Time.timeSinceLevelLoad < 1.5f) return;          // recién salido del edificio (aparece aquí mismo): no avisar
        ultimo = Time.unscaledTime;
        Cartelera.Mostrar(titulo, texto);
    }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.3f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
