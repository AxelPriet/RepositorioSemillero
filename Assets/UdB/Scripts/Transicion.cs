using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Cambio de escena con fundido (pedido 2026-11-13: entrar y salir de los edificios). La pantalla se funde a negro, SOLO
// al terminar se carga la otra escena, se pone al jugador en el PuntoLlegada con el mismo id (y la cámara encima) y la
// pantalla vuelve a aparecer. Vive en un objeto que no se destruye al cambiar de escena. Lo dispara CambioEscena.
public class Transicion : MonoBehaviour
{
    public static bool EnCurso { get; private set; }
    const float Fundido = 0.45f, Pausa = 0.15f;

    static Transicion instancia;
    Image cortina;

    public static void IrA(string escena, string punto)
    {
        if (EnCurso || string.IsNullOrEmpty(escena)) return;
        if (instancia == null)
        {
            var go = new GameObject("Transición (persistente)");
            DontDestroyOnLoad(go);
            instancia = go.AddComponent<Transicion>();
            instancia.CrearCortina();
        }
        instancia.StartCoroutine(instancia.Cambiar(escena, punto));
    }

    IEnumerator Cambiar(string escena, string punto)
    {
        EnCurso = true;
        Congelar(true);
        yield return Fundir(0f, 1f);
        yield return new WaitForSecondsRealtime(Pausa);

        var carga = SceneManager.LoadSceneAsync(escena);
        while (carga != null && !carga.isDone) yield return null;
        yield return null;                                    // un cuadro para que todo haga Awake/Start

        Ubicar(punto);
        Congelar(true);
        yield return null;
        yield return Fundir(1f, 0f);
        Congelar(false);
        EnCurso = false;
    }

    static void Ubicar(string punto)
    {
        var jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;
        PuntoLlegada destino = null;
        foreach (var p in FindObjectsByType<PuntoLlegada>(FindObjectsSortMode.None)) if (p.id == punto) { destino = p; break; }
        if (destino == null) { Debug.LogWarning($"Transición: no hay PuntoLlegada '{punto}' en {SceneManager.GetActiveScene().name}"); return; }
        var llegada = new Vector3(destino.transform.position.x, destino.transform.position.y, jugador.transform.position.z);
        var rb = jugador.GetComponent<Rigidbody2D>();
        if (rb != null) { rb.position = llegada; rb.linearVelocity = Vector2.zero; }
        jugador.transform.position = llegada;
        Physics2D.SyncTransforms();
        var cam = Camera.main;
        if (cam != null)
        {
            var seguir = cam.GetComponent<CameraFollow>();
            if (seguir != null && seguir.Target == null) seguir.Target = jugador.transform;
            var off = seguir != null ? seguir.Offset : new Vector3(0f, 0f, cam.transform.position.z - llegada.z);
            cam.transform.position = llegada + off;
        }
    }

    static void Congelar(bool si)
    {
        var jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;
        var mov = jugador.GetComponent<PlayerMovement>();
        if (mov != null) mov.enabled = !si;
        var rb = jugador.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    IEnumerator Fundir(float desde, float hasta)
    {
        cortina.gameObject.SetActive(true);
        for (float t = 0f; t < Fundido; t += Time.unscaledDeltaTime)
        {
            var c = Color.black; c.a = Mathf.Lerp(desde, hasta, Mathf.SmoothStep(0f, 1f, t / Fundido)); cortina.color = c;
            yield return null;
        }
        var fin = Color.black; fin.a = hasta; cortina.color = fin;
        if (hasta <= 0f) cortina.gameObject.SetActive(false);
    }

    void CrearCortina()
    {
        var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32001;
        gameObject.AddComponent<CanvasScaler>();
        var img = new GameObject("Cortina", typeof(RectTransform), typeof(Image));
        img.transform.SetParent(transform, false);
        var rt = img.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
        cortina = img.GetComponent<Image>(); cortina.raycastTarget = false;
        var c = Color.black; c.a = 0f; cortina.color = c;
        img.SetActive(false);
    }
}