using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Panel Proximidad (solo texto)")]
    [SerializeField] private GameObject proximidadPanel;
    [SerializeField] private TextMeshProUGUI proximidadText;

    [Header("Panel Interacción (nombre + texto + skip)")]
    [SerializeField] private GameObject interaccionPanel;
    [SerializeField] private TextMeshProUGUI interaccionNombreText;
    [SerializeField] private TextMeshProUGUI interaccionDialogueText;
    [SerializeField] private GameObject skipIndicator;
    [SerializeField] private GameObject advanceIndicator;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float skipDelay = 2.5f;

    public bool IsActive => isDialogueActive;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool canSkip = false;
    private System.Action onComplete;
    private Coroutine typingCoroutine;
    private Coroutine skipTimerCoroutine;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        proximidadPanel.SetActive(false);
        interaccionPanel.SetActive(false);
        if (skipIndicator) skipIndicator.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);

        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isTyping) CompleteTyping();
            else ShowNextLine();
        }

        if (canSkip && Keyboard.current.spaceKey.wasPressedThisFrame)
            ShowNextLine();
    }

    public void ShowDialogue(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        proximidadPanel.SetActive(true);
        interaccionPanel.SetActive(false);
        typingCoroutine = StartCoroutine(TypeLineProximidad(text));
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        proximidadPanel.SetActive(false);
        isDialogueActive = false;
    }

    private IEnumerator TypeLineProximidad(string line)
    {
        proximidadText.text = "";
        foreach (char c in line)
        {
            proximidadText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void StartDialogue(string npcName, string[] lines, System.Action onDialogueComplete = null)
    {
        if (isDialogueActive) return;

        currentLines = lines;
        currentLineIndex = 0;
        onComplete = onDialogueComplete;
        isDialogueActive = true;

        if (interaccionNombreText) interaccionNombreText.text = npcName;
        interaccionPanel.SetActive(true);
        proximidadPanel.SetActive(false);
        if (skipIndicator) skipIndicator.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);

        if (playerMovement != null) playerMovement.SetMovementEnabled(false);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex >= currentLines.Length) { EndDialogue(); return; }
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLineInteraccion(currentLines[currentLineIndex]));
    }

    private IEnumerator TypeLineInteraccion(string line)
    {
        isTyping = true;
        canSkip = false;

        if (skipIndicator) skipIndicator.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);

        float startTime = Time.time;
        interaccionDialogueText.text = "";

        foreach (char c in line)
        {
            interaccionDialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        float elapsed = Time.time - startTime;
        float remaining = skipDelay - elapsed;

        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        canSkip = true;
        if (skipIndicator) skipIndicator.SetActive(true);
        if (advanceIndicator) advanceIndicator.SetActive(true);
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        interaccionDialogueText.text = currentLines[currentLineIndex];
        isTyping = false;

        if (skipTimerCoroutine != null) StopCoroutine(skipTimerCoroutine);
        skipTimerCoroutine = StartCoroutine(EsperarYMostrarIndicadores());
    }

    private IEnumerator EsperarYMostrarIndicadores()
    {
        yield return new WaitForSeconds(skipDelay);
        canSkip = true;
        if (skipIndicator) skipIndicator.SetActive(true);
        if (advanceIndicator) advanceIndicator.SetActive(true);
    }

    private void ShowNextLine()
    {
        if (!isDialogueActive || isTyping) return;
        currentLineIndex++;
        canSkip = false;
        if (skipIndicator) skipIndicator.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);
        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (skipTimerCoroutine != null) StopCoroutine(skipTimerCoroutine);

        isDialogueActive = false;
        isTyping = false;
        canSkip = false;

        interaccionPanel.SetActive(false);
        if (skipIndicator) skipIndicator.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);

        if (playerMovement != null) playerMovement.SetMovementEnabled(true);

        onComplete?.Invoke();
        onComplete = null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isDialogueActive = false;
        isTyping = false;
        canSkip = false;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (skipTimerCoroutine != null) StopCoroutine(skipTimerCoroutine);

        interaccionPanel.SetActive(false);
        proximidadPanel.SetActive(false);
        if (skipIndicator) skipIndicator.SetActive(false);
        if (advanceIndicator) advanceIndicator.SetActive(false);

        onComplete = null;

        playerMovement = FindFirstObjectByType<PlayerMovement>();

        bool hayPendiente = GuideManager.Instance != null && GuideManager.Instance.TienePendiente;
        if (playerMovement != null && !hayPendiente)
            playerMovement.SetMovementEnabled(true);
    }
}