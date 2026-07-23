using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MinijuegoTutorial : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoInstruccion;
    [SerializeField] private TextMeshProUGUI textoContador;

    [Header("Configuración")]
    [SerializeField] private int presionesNecesarias = 5;

    private int presiones = 0;
    private bool activo = false;
    private TutorialGuide guia;

    private void Start()
    {
        guia = FindFirstObjectByType<TutorialGuide>();
    }

    private void OnEnable()
    {
        presiones = 0;
        activo = true;

        if (textoInstruccion)
            textoInstruccion.text = "¡Saluda a tus compañeros!\nPresiona SPACE para saludar";
        ActualizarContador();
    }

    private void Update()
    {
        if (!activo) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            presiones++;
            ActualizarContador();

            if (presiones >= presionesNecesarias)
                StartCoroutine(Completar());
        }
    }

    private void ActualizarContador()
    {
        if (textoContador)
            textoContador.text = $"{presiones}/{presionesNecesarias}";
    }

    private IEnumerator Completar()
    {
        activo = false;
        if (textoInstruccion)
            textoInstruccion.text = "¡Bien hecho! Ya estás listo para el recorrido.";

        yield return new WaitForSeconds(1.5f);

        guia?.NotificarMinijuegoCompletado();
    }
}