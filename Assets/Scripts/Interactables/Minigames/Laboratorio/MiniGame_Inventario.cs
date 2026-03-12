using UnityEngine;
using TMPro;
using System.Collections;
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

    private bool juegoCompletado = false;

    private void Start()
    {
        ActualizarUI();
        textoResultado.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (juegoCompletado) return;

        // Verificar constantemente si ya se completó
        if (TodosLosSlotsLlenos())
        {
            StartCoroutine(GanarJuego());
        }
    }

    private bool TodosLosSlotsLlenos()
    {
        SlotEstante[] slots = FindObjectsByType<SlotEstante>(FindObjectsSortMode.None);

        foreach (var slot in slots)
        {
            if (!slot.EstaLleno())
                return false;
        }

        return true;
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;

        textoInstrucciones.text = "¡INVENTARIO ORGANIZADO!";
        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡Mini juego completado!";

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoInstrucciones.text = "Organiza los implementos para llenar todos los espacios";
        textoCapacidad.text = "";
        textoResultado.gameObject.SetActive(false);
    }
}