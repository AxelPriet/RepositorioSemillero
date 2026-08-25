using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class MinijuegoMochila : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int totalObjetos = 3;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    [Header("Cremallera")]
    [SerializeField] private Image imagenCremallera;
    [SerializeField] private Sprite[] fotogramasCremallera;
    [SerializeField] private GameObject panelCremallera;
    [SerializeField] private TextMeshProUGUI textoInstruccion;

    [Header("UI Arrastre")]
    [SerializeField] private GameObject panelArrastre;

    private int objetosColocados = 0;
    private int fotogramaActual = 0;
    private bool faseArrastre = true;
    private bool minijuegoCompletado = false;
    private PlayerControls playerControls;

    private void Start()
    {
        playerControls = InputHandler.Instance?.GetControls();
        if (playerControls != null)
        {
            playerControls.Gameplay.Compress.performed += OnCompress;
            playerControls.Gameplay.Enable();
        }

        panelCremallera.SetActive(false);
        panelArrastre.SetActive(true);

        if (fotogramasCremallera.Length > 0)
            imagenCremallera.sprite = fotogramasCremallera[0];
    }

    private void OnDestroy()
    {
        if (playerControls != null)
            playerControls.Gameplay.Compress.performed -= OnCompress;
    }

    private void OnCompress(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        AvanzarCremallera();
    }

    public void RegistrarObjetoColocado()
    {
        objetosColocados++;
        if (objetosColocados >= totalObjetos)
            IniciarFaseCremallera();
    }

    private void IniciarFaseCremallera()
    {
        faseArrastre = false;
        panelArrastre.SetActive(false);
        panelCremallera.SetActive(true);
        if (textoInstruccion)
            textoInstruccion.text = "¡Presiona Space para cerrar la mochila!";
    }

    private void AvanzarCremallera()
    {
        if (faseArrastre || minijuegoCompletado) return;

        fotogramaActual++;

        if (fotogramaActual >= fotogramasCremallera.Length)
        {
            fotogramaActual = fotogramasCremallera.Length - 1;
            imagenCremallera.sprite = fotogramasCremallera[fotogramaActual];
            StartCoroutine(CompletarMinijuego());
            return;
        }

        imagenCremallera.sprite = fotogramasCremallera[fotogramaActual];
    }

    private IEnumerator CompletarMinijuego()
    {
        minijuegoCompletado = true;
        TutorialGuide guia = FindFirstObjectByType<TutorialGuide>();
        guia?.NotificarMinijuegoCompletado();

        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaPrincipal);
    }
}