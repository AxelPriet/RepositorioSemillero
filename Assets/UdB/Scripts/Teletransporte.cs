using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Zona de teletransporte (TP) con fundido: cuando el jugador entra al trigger, la pantalla se funde a negro, SOLO al
// terminar el fundido se mueve al jugador (y a la cámara) al destino, y luego la pantalla vuelve a aparecer. Mientras
// dura no se puede mover ni disparar otro TP. Va en un objeto con un Collider2D marcado como Is Trigger.
[RequireComponent(typeof(Collider2D))]
public class Teletransporte : MonoBehaviour
{
    [Tooltip("Punto de llegada (fuera de cualquier otra zona de TP)")]
    public Transform destino;
    [Tooltip("Segundos del fundido a negro (y otros tantos para volver)")]
    public float duracionFundido = 0.45f;
    [Tooltip("Segundos en negro entre el fundido de salida y el de entrada")]
    public float pausaEnNegro = 0.15f;
    public Color colorFundido = Color.black;

    static bool enCurso;          // un solo TP a la vez en toda la escena
    static Image cortina;         // imagen a pantalla completa, compartida por todos los TP

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (enCurso || destino == null || !otro.CompareTag("Player")) return;
        StartCoroutine(Teletransportar(otro.attachedRigidbody != null ? otro.attachedRigidbody.gameObject : otro.gameObject));
    }

    IEnumerator Teletransportar(GameObject jugador)
    {
        enCurso = true;
        var rb = jugador.GetComponent<Rigidbody2D>();
        var mov = jugador.GetComponent<PlayerMovement>();
        if (mov != null) mov.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        var c = Cortina();
        yield return Fundir(c, 0f, 1f);                          // 1) fundido a negro completo
        yield return new WaitForSecondsRealtime(pausaEnNegro);

        Vector3 llegada = new Vector3(destino.position.x, destino.position.y, jugador.transform.position.z);   // 2) recién ahora el TP
        if (rb != null) { rb.position = llegada; rb.linearVelocity = Vector2.zero; }
        jugador.transform.position = llegada;
        Physics2D.SyncTransforms();
        var cam = Camera.main;
        if (cam != null)
        {
            var seguir = cam.GetComponent<CameraFollow>();
            var off = seguir != null ? seguir.Offset : new Vector3(0f, 0f, cam.transform.position.z - llegada.z);
            cam.transform.position = llegada + off;               // la cámara llega de una vez, sin barrido
        }
        yield return null;

        yield return Fundir(c, 1f, 0f);                          // 3) vuelve la imagen
        if (mov != null) mov.enabled = true;
        enCurso = false;
    }

    IEnumerator Fundir(Image c, float desde, float hasta)
    {
        c.gameObject.SetActive(true);
        for (float t = 0f; t < duracionFundido; t += Time.unscaledDeltaTime)
        {
            var col = colorFundido; col.a = Mathf.Lerp(desde, hasta, Mathf.SmoothStep(0f, 1f, t / duracionFundido)); c.color = col;
            yield return null;
        }
        var fin = colorFundido; fin.a = hasta; c.color = fin;
        if (hasta <= 0f) c.gameObject.SetActive(false);
    }

    Image Cortina()
    {
        if (cortina != null) return cortina;
        var lienzo = new GameObject("Fundido TP", typeof(Canvas), typeof(CanvasScaler));
        var canvas = lienzo.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32000;
        var img = new GameObject("Cortina", typeof(RectTransform), typeof(Image));
        img.transform.SetParent(lienzo.transform, false);
        var rt = img.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
        cortina = img.GetComponent<Image>(); cortina.raycastTarget = false;
        var col = colorFundido; col.a = 0f; cortina.color = col;
        return cortina;
    }

    void OnDisable() { if (enCurso) { enCurso = false; if (cortina != null) cortina.gameObject.SetActive(false); } }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.35f);
        if (col != null) Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        if (destino != null) { Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.9f); Gizmos.DrawLine(transform.position, destino.position); Gizmos.DrawWireSphere(destino.position, 0.4f); }
    }
}