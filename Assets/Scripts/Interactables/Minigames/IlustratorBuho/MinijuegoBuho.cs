using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinijuegoBuho : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    [SerializeField] private int totalPiezas = 6;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private int piezasColocadas = 0;
    private bool minijuegoCompletado = false;

    private void Start()
    {
        textoPuntuacion.text = $"0/{totalPiezas}";
        textoInstrucciones.text = "Arrastra las piezas a la silueta";
    }

    public void PiezaColocada()
    {
        if (minijuegoCompletado) return;

        piezasColocadas++;
        textoPuntuacion.text = $"{piezasColocadas}/{totalPiezas}";

        if (piezasColocadas >= totalPiezas)
        {
            StartCoroutine(CompletarMinijuego());
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        textoInstrucciones.text = "¡BÚHO COMPLETADO!";
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}