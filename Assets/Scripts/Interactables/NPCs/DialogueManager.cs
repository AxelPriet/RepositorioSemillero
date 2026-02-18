using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private float typingTime = 0.03f;
    [SerializeField] private float stayDuration = 2f;

    private Coroutine typingCoroutine;
    private bool isTyping;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = string.Empty;
        isTyping = true;

        foreach (char ch in line)
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }

        isTyping = false;
    }

    public void FinishAndHide()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(stayDuration);
        dialoguePanel.SetActive(false);
    }
}

