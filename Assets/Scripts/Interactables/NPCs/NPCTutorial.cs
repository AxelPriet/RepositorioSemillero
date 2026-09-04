using UnityEngine;

public class NPCTutorial : MonoBehaviour
{
    [SerializeField] private string npcID = "NPC1";

    private NPCProximidad proximidad;
    private NPCInteraccion interaccion;

    private void Start()
    {
        proximidad = GetComponent<NPCProximidad>();
        interaccion = GetComponent<NPCInteraccion>();

        if (proximidad != null)
            proximidad.OnDialogueTriggered += NotifyTutorial;
        else if (interaccion != null)
            interaccion.OnInteracted += NotifyTutorial;
        else
            Debug.LogWarning("NPCTutorial: No se encontró NPCProximidad ni NPCInteraccion");
    }

    private void NotifyTutorial()
    {
        TutorialGuide guia = FindFirstObjectByType<TutorialGuide>();
        if (guia != null)
            guia.NotificarNPC(npcID);
    }
}
