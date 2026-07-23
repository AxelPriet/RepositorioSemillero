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

    [Header("Puertas (desactivadas al inicio)")]
    [SerializeField] private GameObject puertaZona1; // aparece al terminar zona 1
    [SerializeField] private GameObject puertaZona2; // aparece al terminar zona 2
    [SerializeField] private GameObject puertaZona3; // aparece al terminar zona 3

    [Header("NPCs Zona 2")]
    [SerializeField] private GameObject npc1;
    [SerializeField] private GameObject npc2;

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

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        tutorialDialoguePanel.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);
        if (guideCharacter) guideCharacter.SetActive(false);

        // Puertas cerradas al inicio
        if (puertaZona1) puertaZona1.SetActive(false);
        if (puertaZona2) puertaZona2.SetActive(false);
        if (puertaZona3) puertaZona3.SetActive(false);

        if (minijuegoTutorialPanel) minijuegoTutorialPanel.SetActive(false);

        StartCoroutine(FlujoTutorial());
    }

    private void Update()
    {
        if (esperandoInput && Keyboard.current.spaceKey.wasPressedThisFrame)
            esperandoInput = false;
    }

    // Flujo principal 

    private IEnumerator FlujoTutorial()
    {
        // ZONA 1
        BloquearMovimiento();
        yield return new WaitForSeconds(2f);

        MoverGuiaCercaDelJugador();
        guideCharacter.SetActive(true);

        yield return MostrarDialogos(dialogoZona1);

        // Abrir puerta zona 1
        yield return AbrirPuerta(puertaZona1);

        // Esperar que el jugador pase a la zona 2
        yield return new WaitUntil(() => JugadorEnZona("Zona2"));

        // ZONA 2
        BloquearMovimiento();
        yield return new WaitForSeconds(delayEntreEtapas);

        MoverGuiaCercaDelJugador();
        yield return MostrarDialogos(dialogoZona2);

        HabilitarMovimiento();

        // Esperar que interactúe con los dos NPCs
        yield return new WaitUntil(() => npc1Interactuado && npc2Interactuado);

        BloquearMovimiento();
        yield return new WaitForSeconds(delayEntreEtapas);

        // Abrir puerta zona 2
        yield return AbrirPuerta(puertaZona2);

        // Esperar que pase a zona 3
        yield return new WaitUntil(() => JugadorEnZona("Zona3"));

        // ZONA 3 
        BloquearMovimiento();
        yield return new WaitForSeconds(delayEntreEtapas);

        MoverGuiaCercaDelJugador();
        yield return MostrarDialogos(dialogoZona3);

        HabilitarMovimiento();

        // Iniciar minijuego tutorial
        yield return IniciarMinijuegoTutorial();

        // Al terminar el minijuego cargar Main
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nombreEscenaMain);
    }

    // Minijuego simple de tutorial 

    private IEnumerator IniciarMinijuegoTutorial()
    {
        BloquearMovimiento();

        if (minijuegoTutorialPanel) minijuegoTutorialPanel.SetActive(true);

        // Esperar señal de completado desde MinijuegoTutorial
        yield return new WaitUntil(() => minijuegoCompletado);

        if (minijuegoTutorialPanel) minijuegoTutorialPanel.SetActive(false);
    }

    /// <summary>Llamado desde MinijuegoTutorial cuando el jugador lo completa.</summary>
    public void NotificarMinijuegoCompletado() => minijuegoCompletado = true;

    // Puertas 

    private IEnumerator AbrirPuerta(GameObject puerta)
    {
        if (puerta == null) yield break;

        // Pequeña animación de aparición
        puerta.SetActive(true);
        HabilitarMovimiento();

        // Efecto de escala desde 0 a 1
        puerta.transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            puerta.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        puerta.transform.localScale = Vector3.one;
    }

    // Detección de zona por trigger

    private string zonaActual = "Zona1";

    public void EntrarEnZona(string nombreZona) => zonaActual = nombreZona;

    private bool JugadorEnZona(string zona) => zonaActual == zona;

    // Diálogos

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

    // Notificaciones desde NPCs

    public void NotificarNPC1() => npc1Interactuado = true;
    public void NotificarNPC2() => npc2Interactuado = true;

    // Movimiento

    private void BloquearMovimiento()  => playerMovement?.SetMovementEnabled(false);
    private void HabilitarMovimiento() => playerMovement?.SetMovementEnabled(true);

    private void MoverGuiaCercaDelJugador()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && guideCharacter != null)
            guideCharacter.transform.position =
                player.transform.position + new Vector3(1.5f, 0f, 0f);
    }
}