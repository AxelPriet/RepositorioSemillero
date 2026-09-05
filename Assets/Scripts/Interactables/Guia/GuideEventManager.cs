using System.Collections.Generic;
using UnityEngine;

public class GuideEventManager : MonoBehaviour
{
    private HashSet<string> shownEvents = new HashSet<string>();
    private Queue<string> pendingDialogues = new Queue<string>();

    public System.Action<string> OnDialogueShown;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool EventoMostrado(string dialogueID) => shownEvents.Contains(dialogueID);

    public void RegistrarEventoMostrado(string dialogueID)
    {
        if (!shownEvents.Contains(dialogueID))
            shownEvents.Add(dialogueID);
    }

    public void AgregarPendiente(string dialogueID)
    {
        if (!pendingDialogues.Contains(dialogueID))
            pendingDialogues.Enqueue(dialogueID);
    }

    public string ObtenerSiguientePendiente()
    {
        return pendingDialogues.Count > 0 ? pendingDialogues.Dequeue() : null;
    }

    public bool TienePendientes => pendingDialogues.Count > 0;

    public void LimpiarPendientes() => pendingDialogues.Clear();
}