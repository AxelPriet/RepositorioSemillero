using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private GuideUIManager uiManager;
    [SerializeField] private GuideEventManager eventManager;

    [Header("Personaje Guía (FollowPlayer)")]
    [SerializeField] private GameObject guideCharacter; 

    [Header("Diálogos (ScriptableObjects)")]
    [SerializeField] private GuideDialogueSO[] allDialogues;

    [Header("Configuración")]
    [SerializeField] private string escenaJuego = "Main";

    private bool isPetModeActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (uiManager == null)
            uiManager = GetComponent<GuideUIManager>();

        if (eventManager == null)
            eventManager = GetComponent<GuideEventManager>();

        if (guideCharacter != null)
            guideCharacter.SetActive(false);
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
        if (scene.name == escenaJuego && eventManager.TienePendientes)
        {
            StartCoroutine(MostrarPendientes());
        }
    }

    private IEnumerator MostrarPendientes()
    {
        yield return new WaitForSeconds(0.5f);

        while (eventManager.TienePendientes)
        {
            string dialogueID = eventManager.ObtenerSiguientePendiente();
            TriggerEvent(dialogueID);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void TriggerEvent(string dialogueID, System.Action onComplete = null)
    {
        if (eventManager.EventoMostrado(dialogueID))
        {
            onComplete?.Invoke();
            return;
        }

        GuideDialogueSO dialogue = System.Array.Find(allDialogues, d => d.DialogueID == dialogueID);
        if (dialogue == null)
        {
            Debug.LogWarning($"Diálogo '{dialogueID}' no encontrado");
            onComplete?.Invoke();
            return;
        }

        if (dialogue.MostrarUnaVez)
            eventManager.RegistrarEventoMostrado(dialogueID);

        uiManager.MostrarDialogo(dialogue.Lines, () => {
            if (!isPetModeActive)
                uiManager.OcultarGuia();
            onComplete?.Invoke();
        });
    }

    public void SetPendingDialogue(string dialogueID)
    {
        eventManager.AgregarPendiente(dialogueID);
    }

    public bool TienePendientes()
    {
        return eventManager != null && eventManager.TienePendientes;
    }

    public void ActivarPetMode()
    {
        if (guideCharacter == null) return;

        isPetModeActive = true;
        guideCharacter.SetActive(true);

        if (guideCharacter.GetComponent<FollowPlayer>() == null)
            guideCharacter.AddComponent<FollowPlayer>();
    }

    public void DesactivarPetMode()
    {
        isPetModeActive = false;
        if (guideCharacter != null)
            guideCharacter.SetActive(false);
    }

    public bool EventoMostrado(string dialogueID) => eventManager.EventoMostrado(dialogueID);


    public void MostrarBienvenida() => TriggerEvent("BienvenidaInicio");
    public void MostrarExplicacion(string minijuegoID) => TriggerEvent($"Explicacion{minijuegoID}");
    public void MostrarFin(string minijuegoID) => TriggerEvent($"Fin{minijuegoID}");
}