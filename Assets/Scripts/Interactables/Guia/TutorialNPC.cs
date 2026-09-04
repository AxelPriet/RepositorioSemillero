using UnityEngine;
using EclipseGames.Player.Interaction;

public class TutorialNPC : MonoBehaviour, IInteractuable
{
    public enum TipoNPC { NPC1, NPC2 }

    [SerializeField] private TipoNPC tipo;
    [SerializeField] private string prompt = "E";

    [TextArea(2, 4)]
    [SerializeField] private string textoDialogo;

    private bool interactuado = false;
    private TutorialGuide guia;

    private void Start()
    {
        guia = FindFirstObjectByType<TutorialGuide>();
    }

    public void Interactuar()
    {
        if (interactuado) return;
        interactuado = true;

        DialogueManager.Instance?.ShowDialogue(textoDialogo);

        string npcID = (tipo == TipoNPC.NPC1) ? "NPC1" : "NPC2";
        guia?.NotificarNPC(npcID);
    }

    public string GetPrompt() => prompt;
    public bool PuedeInteractuar() => !interactuado;
    public Transform GetTransform() => transform;
}