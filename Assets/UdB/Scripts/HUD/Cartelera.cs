using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Cartelera del HUD (pedido 2026-11-17): un letrero que baja desde arriba de la pantalla, dice dónde está el jugador o a qué
// entrada llega, espera unos segundos y se vuelve a esconder. La usan Lugar (zonas con nombre) y AvisoEntrada.
// Va en el panel (RectTransform anclado arriba al centro). Fuera de la pantalla queda en "yEscondida".
public class Cartelera : MonoBehaviour
{
    public static Cartelera Instancia { get; private set; }

    [Tooltip("Texto pequeño de arriba (\"Estás en\", \"Entrada\")")]
    public Text titulo;
    [Tooltip("Texto grande (el nombre del lugar)")]
    public Text texto;
    [Tooltip("Segundos que se queda a la vista")]
    public float segundos = 3f;
    [Tooltip("Segundos que tarda en bajar o en subir")]
    public float duracionAnimacion = 0.35f;
    [Tooltip("Posición Y (anclada arriba) cuando se ve")]
    public float yVisible = -12f;
    [Tooltip("Posición Y cuando está escondida, por encima del borde de la pantalla")]
    public float yEscondida = 140f;

    RectTransform rt;
    Coroutine rutina;
    float visible;                 // 0 = escondida, 1 = abajo del todo
    string ultimo;                 // nombre que muestra ahora

    void Awake()
    {
        Instancia = this;
        rt = (RectTransform)transform;
        Poner(0f);
    }

    void OnDestroy() { if (Instancia == this) Instancia = null; }

    public static void Mostrar(string tituloArriba, string nombre)
    {
        if (Instancia != null) Instancia.MostrarAqui(tituloArriba, nombre);
    }

    public void MostrarAqui(string tituloArriba, string nombre)
    {
        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(Ciclo(tituloArriba, nombre));
    }

    IEnumerator Ciclo(string tituloArriba, string nombre)
    {
        if (visible > 0.01f && ultimo != nombre)   // ya se veía otro letrero: sube y cambia
        {
            yield return Mover(visible, 0f, duracionAnimacion * 0.6f);
        }
        ultimo = nombre;
        if (titulo != null) titulo.text = Pixel(tituloArriba);
        if (texto != null) Ajustar(Pixel(nombre));
        yield return Mover(visible, 1f, duracionAnimacion);
        yield return new WaitForSecondsRealtime(segundos);
        yield return Mover(1f, 0f, duracionAnimacion);
        rutina = null;
    }

    IEnumerator Mover(float desde, float hasta, float dur)
    {
        if (dur <= 0f) { Poner(hasta); yield break; }
        for (float t = 0f; t < dur; t += Time.unscaledDeltaTime)
        {
            float k = t / dur;
            k = hasta > desde ? 1f - (1f - k) * (1f - k) * (1f - k) : k * k;   // baja frenando, sube acelerando
            Poner(Mathf.Lerp(desde, hasta, k));
            yield return null;
        }
        Poner(hasta);
    }

    /// <summary>
    /// La fuente pixelada (Perfect DOS VGA 437) tiene los caracteres en las posiciones de la página de códigos 437:
    /// por ejemplo, la "á" está donde Unicode tiene U+00A0. Convierte el texto para que las tildes y la ñ salgan bien.
    /// Á, Í, Ó y Ú no existen en esa página: salen sin tilde.
    /// </summary>
    public static string Pixel(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        var sb = new System.Text.StringBuilder(s.Length);
        foreach (var c in s)
            switch (c)
            {
                case 'á': sb.Append(' '); break; case 'é': sb.Append('\u0082'); break; case 'í': sb.Append('¡'); break;
                case 'ó': sb.Append('¢'); break; case 'ú': sb.Append('£'); break; case 'ñ': sb.Append('¤'); break;
                case 'Ñ': sb.Append('¥'); break; case 'É': sb.Append('\u0090'); break; case 'ü': sb.Append('\u0081'); break;
                case 'Ü': sb.Append('\u009A'); break; case '¿': sb.Append('¨'); break; case '¡': sb.Append('­'); break;
                case 'Á': sb.Append('A'); break; case 'Í': sb.Append('I'); break; case 'Ó': sb.Append('O'); break; case 'Ú': sb.Append('U'); break;
                default: sb.Append(c); break;
            }
        return sb.ToString();
    }

    // Si el nombre no cabe en una línea, lo parte en dos por el espacio más cercano a la mitad y el letrero crece hacia abajo.
    float altoBase = -1f, altoTextoBase;
    void Ajustar(string s)
    {
        var tr = texto.rectTransform;
        if (altoBase < 0f) { altoBase = rt.sizeDelta.y; altoTextoBase = tr.sizeDelta.y; }
        texto.text = s;
        int lineas = 1;
        if (texto.preferredWidth > tr.rect.width && s.IndexOf(' ') > 0)
        {
            int mejor = -1;
            for (int i = 0; i < s.Length; i++) if (s[i] == ' ' && (mejor < 0 || Mathf.Abs(i - s.Length / 2) < Mathf.Abs(mejor - s.Length / 2))) mejor = i;
            texto.text = s.Substring(0, mejor) + "\n" + s.Substring(mejor + 1);
            lineas = 2;
        }
        float extra = (lineas - 1) * texto.fontSize;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, altoBase + extra);
        tr.sizeDelta = new Vector2(tr.sizeDelta.x, altoTextoBase + extra);
        Poner(visible);
    }

    void Poner(float v)
    {
        visible = v;
        if (rt == null) rt = (RectTransform)transform;
        float escondida = Mathf.Max(yEscondida, rt.sizeDelta.y + 16f);   // que no asome aunque haya crecido
        var p = rt.anchoredPosition; p.y = Mathf.Lerp(escondida, yVisible, v); rt.anchoredPosition = p;
    }
}
