using UnityEngine;

// Flecha animada que marca una entrada (pedido 2026-11-17): sube y baja hacia donde apunta, con un pulso suave, y se
// desvanece cuando el jugador está lejos. Va en un objeto con SpriteRenderer (sprite hud_flecha, que apunta hacia abajo);
// gira el objeto para que apunte a la puerta.
[RequireComponent(typeof(SpriteRenderer))]
public class FlechaEntrada : MonoBehaviour
{
    [Tooltip("Cuánto se mueve hacia la puerta y de vuelta (unidades)")]
    public float amplitud = 0.25f;
    [Tooltip("Vueltas por segundo")]
    public float velocidad = 1.4f;
    [Tooltip("Distancia a la que empieza a verse")]
    public float distanciaVisible = 14f;
    [Tooltip("Distancia desde la que se ve entera")]
    public float distanciaPlena = 9f;

    Vector3 basePos;
    SpriteRenderer sr;
    Transform jugador;
    float fase;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        basePos = transform.localPosition;
        fase = (transform.position.x * 0.37f + transform.position.y * 0.21f) % 1f;   // que no se muevan todas a la vez
    }

    void Update()
    {
        float t = (Time.time * velocidad + fase) * Mathf.PI * 2f;
        float s = (Mathf.Sin(t) + 1f) * 0.5f;                               // 0..1
        s = s * s * (3f - 2f * s);                                           // más tiempo en los extremos: rebote suave
        Vector3 haciaPuerta = transform.parent != null ? transform.parent.InverseTransformDirection(-transform.up) : -transform.up;
        transform.localPosition = basePos + haciaPuerta * (amplitud * s);
        float pulso = 1f + 0.06f * Mathf.Sin(t * 2f);
        transform.localScale = new Vector3(pulso, pulso, 1f);

        if (jugador == null) { var j = GameObject.FindGameObjectWithTag("Player"); if (j != null) jugador = j.transform; }
        float a = 1f;
        if (jugador != null)
        {
            float d = Vector2.Distance(jugador.position, transform.position);
            a = Mathf.Clamp01((distanciaVisible - d) / Mathf.Max(0.01f, distanciaVisible - distanciaPlena));
        }
        var c = sr.color; c.a = a; sr.color = c;
    }
}
