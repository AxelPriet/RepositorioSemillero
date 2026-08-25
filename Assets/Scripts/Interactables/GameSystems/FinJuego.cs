using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FinJuego : MonoBehaviour
{
    public static FinJuego Instance;

    [Header("UI Final")]
    [SerializeField] private GameObject panelFinal;
    [SerializeField] private Animator animadorFinal; 
    [SerializeField] private Image fadeImage;

    [Header("Configuración")]
    [SerializeField] private float delayAntesDeFade = 1f;
    [SerializeField] private float duracionFade = 1.5f;
    //[SerializeField] private string escenaCreditos = "Creditos";

    private bool finalActivado = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (panelFinal) panelFinal.SetActive(false);
        if (fadeImage)
        {
            fadeImage.gameObject.SetActive(false);
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void ActivarFinal()
    {
        if (finalActivado) return;
        finalActivado = true;
        StartCoroutine(SecuenciaFinal());
    }

    private IEnumerator SecuenciaFinal()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        player?.SetMovementEnabled(false);

        yield return new WaitForSeconds(delayAntesDeFade);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0f;
            while (t < duracionFade)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t / duracionFade));
                yield return null;
            }
        }

        if (panelFinal) panelFinal.SetActive(true);
        if (animadorFinal) animadorFinal.SetTrigger("Iniciar");

        if (fadeImage != null)
        {
            float t = 0f;
            while (t < duracionFade)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(1f - t / duracionFade));
                yield return null;
            }
            fadeImage.gameObject.SetActive(false);
        }
    }
}