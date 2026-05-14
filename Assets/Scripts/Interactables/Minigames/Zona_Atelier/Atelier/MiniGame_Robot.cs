using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniGame_Robot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;

    [Header("Configuración")]
    [SerializeField] private int totalPiezas = 5;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private int piezasColocadas = 0;
    private bool juegoCompletado = false;
    private List<string> piezasColocadasList = new List<string>();
    [SerializeField] private int minigameIndex;


    private void Start()
    {
        ActualizarUI();
    }

    public void PiezaColocada(string nombrePieza)
    {
        if (juegoCompletado) return;

        if (!piezasColocadasList.Contains(nombrePieza))
        {
            piezasColocadasList.Add(nombrePieza);
            piezasColocadas++;

            textoPuntuacion.text = $"Piezas: {piezasColocadas}/{totalPiezas}";

            if (piezasColocadas >= totalPiezas)
            {
                StartCoroutine(CompletarRobot());
            }
        }
    }

    private IEnumerator CompletarRobot()
    {
        juegoCompletado = true;
        textoPuntuacion.gameObject.SetActive(false);


        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinAtelier);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoPuntuacion.text = $"Piezas: 0/{totalPiezas}";
    }
}