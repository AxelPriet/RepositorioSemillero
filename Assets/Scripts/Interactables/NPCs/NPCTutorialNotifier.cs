using UnityEngine;

public class NPCTutorialNotifier : MonoBehaviour
{
    [SerializeField] public NPCInteraction interaction;
    [SerializeField] public string idNPCTutorial = "NPC1";

    public void SetInteraction(NPCInteraction newInteraction) => interaction = newInteraction;
    public void SetID(string id) => idNPCTutorial = id;

    private void Start()
    {
        if (interaction != null)
            interaction.OnInteracted += NotifyTutorial;
    }

    private void NotifyTutorial()
    {
        TutorialGuide guia = FindFirstObjectByType<TutorialGuide>();
        if (guia == null) return;
        guia.NotificarNPC(idNPCTutorial);
    }
}