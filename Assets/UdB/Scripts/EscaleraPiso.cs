using UnityEngine;
using UnityEngine.SceneManagement;

// Escalera entre pisos de un mismo edificio (pedido 2026-11-14). Zona (trigger) al fondo de un tramo de escalera: al entrar,
// el jugador pasa al piso de arriba o al de abajo con el mismo fundido de CambioEscena.
//
// No hay que escribir el nombre de la escena destino: sale del nombre de la escena actual, que debe ser
// "<Edificio> - Piso <n>" (por ejemplo "Edificio 3 - Piso 2"). Subir lleva a "<Edificio> - Piso <n+1>" y Bajar a
// "<Edificio> - Piso <n-1>". Por eso la misma escalera se puede copiar tal cual de un piso a otro, o a otro edificio.
//
// En el piso de destino el jugador aparece en el PuntoLlegada cuyo id es:
//   "<torre> (subiendo)"  si llegó subiendo  -> va al pie del tramo que BAJA de ese piso (por ahí acaba de subir)
//   "<torre> (bajando)"   si llegó bajando   -> va al pie del tramo que SUBE de ese piso (por ahí acaba de bajar)
// "torre" distingue las escaleras de un mismo edificio ("Torre izq", "Torre der"...): debe llamarse igual en todos los pisos.
[RequireComponent(typeof(Collider2D))]
public class EscaleraPiso : MonoBehaviour
{
    public enum Sentido { Subir, Bajar }

    public Sentido sentido = Sentido.Subir;
    [Tooltip("Nombre de la torre o escalera; igual en todos los pisos del edificio (\"Torre izq\", \"Torre der\"...)")]
    public string torre = "Torre izq";

    public const string Separador = " - Piso ";

    // "Edificio 3 - Piso 2" -> edificio "Edificio 3", piso 2
    public static bool LeerEscena(string escena, out string edificio, out int piso)
    {
        edificio = null; piso = 0;
        if (string.IsNullOrEmpty(escena)) return false;
        int i = escena.LastIndexOf(Separador, System.StringComparison.Ordinal);
        if (i <= 0 || !int.TryParse(escena.Substring(i + Separador.Length), out piso)) return false;
        edificio = escena.Substring(0, i);
        return true;
    }

    public static string NombreEscena(string edificio, int piso) => edificio + Separador + piso;

    public static string IdLlegada(string torre, Sentido conQueSentidoLlego) => torre + (conQueSentidoLlego == Sentido.Subir ? " (subiendo)" : " (bajando)");

    // Escena y punto de llegada a donde lleva esta escalera, estando en la escena "escenaActual".
    public bool Destino(string escenaActual, out string escena, out string punto)
    {
        escena = punto = null;
        if (!LeerEscena(escenaActual, out var edificio, out var piso)) return false;
        escena = NombreEscena(edificio, piso + (sentido == Sentido.Subir ? 1 : -1));
        punto = IdLlegada(torre, sentido);
        return true;
    }

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (Transicion.EnCurso || !otro.CompareTag("Player")) return;
        var actual = gameObject.scene.name;
        if (!Destino(actual, out var escena, out var punto))
        {
            Debug.LogWarning($"EscaleraPiso: la escena '{actual}' no se llama \"<Edificio>{Separador}<n>\"; no sé a qué piso ir.", this);
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(escena))
        {
            Debug.LogWarning($"EscaleraPiso: no existe la escena '{escena}' (o no está en Build Settings).", this);
            return;
        }
        Transicion.IrA(escena, punto);
    }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = sentido == Sentido.Subir ? new Color(0.2f, 0.9f, 0.4f, 0.45f) : new Color(0.25f, 0.55f, 1f, 0.45f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
