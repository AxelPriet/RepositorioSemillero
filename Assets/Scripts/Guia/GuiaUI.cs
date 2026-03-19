using UnityEngine;
using TMPro;
using System.Collections;

public class GuiaUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoMensaje;
    [SerializeField] private float tiempoPorDefecto = 3f;

    private Coroutine rutinaOcultar;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void MostrarMensaje(string mensaje, float duracion = -1)
    {
        if (rutinaOcultar != null)
            StopCoroutine(rutinaOcultar);

        panel.SetActive(true);
        textoMensaje.text = mensaje;

        if (duracion < 0) duracion = tiempoPorDefecto;
        rutinaOcultar = StartCoroutine(OcultarDespuesDe(duracion));
    }

    private IEnumerator OcultarDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        panel.SetActive(false);
        rutinaOcultar = null;
    }

    public void OcultarInmediato()
    {
        if (rutinaOcultar != null)
            StopCoroutine(rutinaOcultar);
        panel.SetActive(false);
    }
}