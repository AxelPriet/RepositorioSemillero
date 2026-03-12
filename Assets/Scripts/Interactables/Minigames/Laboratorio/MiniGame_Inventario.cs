using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniGame_Inventario : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoCapacidad;
    [SerializeField] private TextMeshProUGUI textoResultado;

    [Header("Configuración")]
    [SerializeField] private int totalImplementos = 7;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private int implementosColocados = 0;
    private bool juegoCompletado = false;
    private int espaciosTotales = 9; 
    private int espaciosUsados = 0;

    private void Start()
    {
        ActualizarUI();
        textoResultado.gameObject.SetActive(false);
    }

    public void ImplementoColocado()
    {
        if (juegoCompletado) return;

        implementosColocados++;
        CalcularEspaciosUsados();

        if (implementosColocados >= totalImplementos)
        {
            if (espaciosUsados == espaciosTotales)
            {
                StartCoroutine(GanarJuego());
            }
        }
    }

    private void CalcularEspaciosUsados()
    {
        espaciosUsados = 0;
        SlotEstante[] slots = FindObjectsByType<SlotEstante>(FindObjectsSortMode.None);

        foreach (var slot in slots)
        {
           
        }

        textoCapacidad.text = $"Espacios: {espaciosUsados}/{espaciosTotales}";
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;

        textoInstrucciones.text = "¡INVENTARIO ORGANIZADO!";
        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡COMPLETASTE!";

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoInstrucciones.text = "Coloca los implementos en los estantes";
        textoCapacidad.text = $"Espacios: 0/{espaciosTotales}";
        textoResultado.gameObject.SetActive(false);
    }
}