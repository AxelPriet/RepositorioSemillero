using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGame_Inventario : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int totalImplementos = 7;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    private bool juegoCompletado = false;
    [SerializeField] private int minigameIndex;


    private void Start()
    {
    }

    private void Update()
    {
        if (juegoCompletado) return;

        if (TodosLosSlotsLlenos())
        {
            StartCoroutine(GanarJuego());
        }
    }

    private bool TodosLosSlotsLlenos()
    {
        SlotEstante[] slots = FindObjectsByType<SlotEstante>(FindObjectsSortMode.None);

        int implementosColocados = 0;

        foreach (var slot in slots)
        {
            if (slot.EstaLleno())
                implementosColocados++;
        }

        return implementosColocados >= totalImplementos;
    }

    private IEnumerator GanarJuego()
    {
        juegoCompletado = true;

        yield return new WaitForSeconds(2f);
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinLaboratorio);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}