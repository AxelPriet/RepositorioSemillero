using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstruccionesIniciales : MonoBehaviour
{
    [Header("Panel de Instrucciones")]
    [SerializeField] private GameObject panelInstrucciones;
    [SerializeField] private TextMeshProUGUI textoTemporizador;

    [Header("Tiempo")]
    [SerializeField] private float tiempoLectura = 10f; 

    [Header("Referencias al Juego")]
    [SerializeField] private GameObject juegoPrincipal; 
    [SerializeField] private MonoBehaviour[] scriptsDelJuego;

    private void Start()
    {
        juegoPrincipal.SetActive(false);
        foreach (var script in scriptsDelJuego)
        {
            script.enabled = false;
        }

        StartCoroutine(CuentaRegresiva());
    }

    private IEnumerator CuentaRegresiva()
    {
        float tiempoRestante = tiempoLectura;

        while (tiempoRestante > 0)
        {
            textoTemporizador.text = $"Comenzando en: {tiempoRestante:F0}s";
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
        }

        textoTemporizador.text = "¡YA!";
        yield return new WaitForSeconds(0.5f);

        juegoPrincipal.SetActive(true);
        foreach (var script in scriptsDelJuego)
        {
            script.enabled = true;
        }

        panelInstrucciones.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StopAllCoroutines();
            juegoPrincipal.SetActive(true);
            foreach (var script in scriptsDelJuego)
            {
                script.enabled = true;
            }
            panelInstrucciones.SetActive(false);
        }
    }
}