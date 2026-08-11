using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialGuide : MonoBehaviour
{
    [Header("Personaje visual del guía")]
    [SerializeField] private GameObject guideCharacter;

    [Header("Panel de diálogo")]
    [SerializeField] private GameObject tutorialDialoguePanel;
    [SerializeField] private TextMeshProUGUI nombreText;
    [SerializeField] private TextMeshProUGUI dialogoText;
    [SerializeField] private GameObject advanceIndicator;

    [Header("Configuración")]
    [SerializeField] private string guideName = "A.A.V.";
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float delayEntreEtapas = 0.5f;

    [Header("Diálogos SO")]
    [SerializeField] private TutorialDialogoSO dialogoZona1;
    [SerializeField] private TutorialDialogoSO dialogoZona2;
    [SerializeField] private TutorialDialogoSO dialogoZona3;

    [Header("Puertas")]
    [SerializeField] private GameObject puertaZona1;
    [SerializeField] private GameObject puertaZona2;
    [SerializeField] private float duracionApparicionPuerta = 1.5f;

    [Header("Minijuego Zona 3")]
    [SerializeField] private GameObject minijuegoTutorialPanel;

    [Header("Escena siguiente")]
    [SerializeField] private string nombreEscenaMain = "Main";

    // Estado
    private PlayerMovement playerMovement;
    private bool esperandoInput = false;
    private bool npc1Interactuado = false;
    private bool npc2Interactuado = false;
    private bool minijuegoCompletado = false;
    private bool jugadorSeMovio = false;
    private bool jugadorCorrio = false;
    private string zonaActual = "Zona1";

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        tutorialDialoguePanel.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);
        if (guideCharacter) guideCharacter.SetActive(false);
        if (puertaZona1) puertaZona1.SetActive(false);
        if (puertaZona2) puertaZona2.SetActive(false);
        if (minijuegoTutorialPanel) minijuegoTutorialPanel.SetActive(false);

        StartCoroutine(FlujoTutorial());
    }

    private void Update()
    {
        if (esperandoInput && Keyboard.current.spaceKey.wasPressedThisFrame)
            esperandoInput = false;

        // Detectar movimiento y correr en Zona 1
        if (zonaActual == "Zona1" && playerMovement != null)
        {
            if (InputHandler.Instance != null)
            {
                Vector2 input = InputHandler.Instance.GetMoveInput();
                if (input != Vector2.zero)
                    jugadorSeMovio = true;
                if (input != Vector2.zero && InputHandler.Instance.IsRunning())
                    jugadorCorrio = true;
            }
        }
    }

    // ── Flujo principal ──────────────────────────────────────

    private IEnumerator FlujoTutorial()
    {
        // ── ZONA 1 ──────────────────────────────────────────
        BloquearMovimiento();
        yield return new WaitForSeconds(2f);

        MoverGuiaCercaDelJugador();
        guideCharacter.SetActive(true);

        yield return MostrarDialogos(dialogoZona1);

        // Dar control para que se mueva y corra
        HabilitarMovimiento();
        yield return new WaitUntil(() => jugadorSeMovio && jugadorCorrio);

        yield return new WaitForSeconds(delayEntreEtapas);
        yield return MostrarPuerta(puertaZona1);

        // Esperar que cruce a Zona 2 via PuertaTransicion
        yield return new WaitUntil(() => JugadorEnZona("Zona2"));

        // ── ZONA 2 ──────────────────────────────────────────
        BloquearMovimiento();
        yield return new WaitForSeconds(delayEntreEtapas);

        MoverGuiaCercaDelJugador();
        yield return MostrarDialogos(dialogoZona2);

        HabilitarMovimiento();

        yield return new WaitUntil(() => npc1Interactuado && npc2Interactuado);

        yield return new WaitForSeconds(delayEntreEtapas);
        yield return MostrarPuerta(puertaZona2);

        // Esperar que cruce a Zona 3 via PuertaTransicion
        yield return new WaitUntil(() => JugadorEnZona("Zona3"));

        // ── ZONA 3 ──────────────────────────────────────────
        BloquearMovimiento();
        yield return new WaitForSeconds(delayEntreEtapas);

        MoverGuiaCercaDelJugador();
        yield return MostrarDialogos(dialogoZona3);

        HabilitarMovimiento();

        // El minijuego siempre está activo — solo esperar que lo complete
        yield return new WaitUntil(() => minijuegoCompletado);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nombreEscenaMain);
    }

    // ── Aparición de puerta con delay ────────────────────────

    private IEnumerator MostrarPuerta(GameObject puerta)
    {
        if (puerta == null) yield break;

        yield return new WaitForSeconds(duracionApparicionPuerta);

        puerta.SetActive(true);
        puerta.transform.localScale = Vector3.zero;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            puerta.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        puerta.transform.localScale = Vector3.one;
    }

    // ── Detección de zona ────────────────────────────────────

    public void EntrarEnZona(string nombreZona)
    {
        zonaActual = nombreZona;
        Debug.Log($"[Tutorial] Zona actual: {nombreZona}");
    }

    private bool JugadorEnZona(string zona) => zonaActual == zona;

    // ── Diálogos ─────────────────────────────────────────────

    private IEnumerator MostrarDialogos(TutorialDialogoSO so)
    {
        if (so == null || so.lineas.Length == 0) yield break;

        tutorialDialoguePanel.SetActive(true);
        if (nombreText) nombreText.text = guideName;

        foreach (string linea in so.lineas)
        {
            if (advanceIndicator) advanceIndicator.SetActive(false);
            yield return EscribirTexto(linea);

            esperandoInput = true;
            if (advanceIndicator) advanceIndicator.SetActive(true);
            yield return new WaitUntil(() => !esperandoInput);
            yield return null;
        }

        tutorialDialoguePanel.SetActive(false);
        if (guideCharacter) guideCharacter.SetActive(false);
    }

    private IEnumerator EscribirTexto(string texto)
    {
        dialogoText.text = "";
        foreach (char c in texto)
        {
            dialogoText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // ── Notificaciones ───────────────────────────────────────

    public void NotificarNPC1() => npc1Interactuado = true;
    public void NotificarNPC2() => npc2Interactuado = true;
    public void NotificarMinijuegoCompletado() => minijuegoCompletado = true;

    // ── Movimiento ───────────────────────────────────────────

    private void BloquearMovimiento()  => playerMovement?.SetMovementEnabled(false);
    private void HabilitarMovimiento() => playerMovement?.SetMovementEnabled(true);

    private void MoverGuiaCercaDelJugador()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && guideCharacter != null)
            guideCharacter.transform.position =
                player.transform.position + new Vector3(1.5f, 0f, 0f);
        if (guideCharacter) guideCharacter.SetActive(true);
    }
}