using UnityEngine;
using TMPro;
using System.Collections;

public class MinijuegoBuho : MonoBehaviour, IInteractuable, IMinigame
{
    [Header("Panel UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    [SerializeField] private TextMeshProUGUI textoFeedback;

    [Header("Configuración")]
    [SerializeField] private int totalPuntos = 6;
    [SerializeField] private int minigameIndex = 6;

    private PlayerControls playerControls;
    private PlayerMovement playerMovement;
    private bool enJuego = false;
    private int piezasColocadas = 0;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void PiezaColocada()
    {
        if (!enJuego) return;

        piezasColocadas++;
        textoFeedback.text = $"¡Bien! {piezasColocadas}/{totalPuntos}";

        if (piezasColocadas >= totalPuntos)
        {
            StartCoroutine(CompletarMinijuego());
        }
    }

    private IEnumerator CompletarMinijuego()
    {
        textoFeedback.text = "¡BÚHO COMPLETADO!";
        textoInstrucciones.text = "¡Felicidades!";
        yield return new WaitForSeconds(1f);
        CompleteMinigame();
    }

    public void StartMinigame()
    {
        enJuego = true;
        piezasColocadas = 0;

        playerControls = InputHandler.Instance.GetControls();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerMovement?.SetCanMove(false);

        panel.SetActive(true);

        textoInstrucciones.text = "Arrastra las piezas a la silueta";
        textoFeedback.text = $"Piezas: 0/{totalPuntos}";

        // Limpiar puntos ocupados
        foreach (var punto in FindObjectsByType<PuntoOcupado>(FindObjectsSortMode.None))
        {
            Destroy(punto);
        }
    }

    public void CompleteMinigame()
    {
        enJuego = false;
        playerMovement?.SetCanMove(true);
        panel.SetActive(false);
        GameProgressManager.Instance.CompleteMinigame();
    }

    public void Interactuar()
    {
        if (!PuedeInteractuar()) return;
        StartMinigame();
    }

    public bool PuedeInteractuar()
    {
        //return GameProgressManager.Instance.CanPlayMinigame(minigameIndex) && !enJuego;
        return !enJuego;
    }

    public string GetPrompt()
    {
        if (!PuedeInteractuar())
            return "Minijuego completado";
        return "Armar Búho";
    }

    public Transform GetTransform() => transform;
    public void FailMinigame() { }
}
