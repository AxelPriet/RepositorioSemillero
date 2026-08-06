using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuertaTransicion : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private string nombreZonaDestino;
    [SerializeField] private Vector2 posicionDestino;

    [Header("Transición")]
    [SerializeField] private float delayAntesDeTeletransportar = 2f;
    [SerializeField] private float duracionFade = 1f;

    [Header("UI Fade")]
    [SerializeField] private Image fadeImage;

    private bool transicionando = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (transicionando) return;

        transicionando = true;
        StartCoroutine(RealizarTransicion(other.gameObject));
    }

    private IEnumerator RealizarTransicion(GameObject player)
    {
        PlayerMovement movimiento = player.GetComponent<PlayerMovement>();
        movimiento?.SetMovementEnabled(false);

        yield return new WaitForSeconds(delayAntesDeTeletransportar);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return Fade(fadeImage, 0f, 1f, duracionFade);
        }

        player.transform.position = posicionDestino;

        // ✅ Línea corregida:
        TutorialGuide guia = FindFirstObjectByType<TutorialGuide>();
        guia?.EntrarEnZona(nombreZonaDestino);

        if (fadeImage != null)
        {
            yield return Fade(fadeImage, 1f, 0f, duracionFade);
            fadeImage.gameObject.SetActive(false);
        }

        movimiento?.SetMovementEnabled(true);
        transicionando = false;
    }

    private IEnumerator Fade(Image img, float desde, float hasta, float duracion)
    {
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(desde, hasta, t / duracion);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        img.color = new Color(0, 0, 0, hasta);
    }
}