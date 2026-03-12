using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniGame_Barras : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoResultado;

    [Header("Configuración")]
    [SerializeField] private int totalBarras = 4;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private int barrasColocadas = 0;
    private bool juegoCompletado = false;
    private List<string> barrasColocadasList = new List<string>();

    private void Start()
    {
        ActualizarUI();
        textoResultado.gameObject.SetActive(false);
    }

    public void BarraColocada(string nombreBarra)
    {
        if (juegoCompletado) return;

        if (!barrasColocadasList.Contains(nombreBarra))
        {
            barrasColocadasList.Add(nombreBarra);
            barrasColocadas++;

            textoPuntuacion.text = $"Barras: {barrasColocadas}/{totalBarras}";

            if (barrasColocadas >= totalBarras)
            {
                StartCoroutine(CompletarJuego());
            }
        }
    }

    private IEnumerator CompletarJuego()
    {
        juegoCompletado = true;

        textoInstrucciones.text = "¡GRÁFICO COMPLETADO!";
        textoPuntuacion.gameObject.SetActive(false);

        textoResultado.gameObject.SetActive(true);
        textoResultado.text = "¡ESTADÍSTICAS ORDENADAS!";

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoPuntuacion.text = $"Barras: 0/{totalBarras}";
        textoInstrucciones.text = "Arrastra cada barra a su categoría";
    }
}