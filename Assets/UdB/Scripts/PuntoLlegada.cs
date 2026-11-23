using UnityEngine;

// Lugar donde aparece el jugador al llegar a esta escena por un CambioEscena con el mismo id (por ejemplo "E2_izq":
// afuera, frente a la entrada izquierda del Edificio 2, en la escena Universidad; y adentro, junto a esa puerta, en la
// escena del Edificio 2). Debe quedar fuera de cualquier zona de cambio de escena.
public class PuntoLlegada : MonoBehaviour
{
    public string id;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 1f, 0.4f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, 0.45f);
        Gizmos.DrawLine(transform.position + Vector3.left * 0.45f, transform.position + Vector3.right * 0.45f);
    }
}
