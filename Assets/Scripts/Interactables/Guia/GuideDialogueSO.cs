using UnityEngine;

[CreateAssetMenu(fileName = "GuideDialogue", menuName = "Guide/Nuevo Dialogo", order = 1)]
public class GuideDialogueSO : ScriptableObject
{
    [Header("Identificador")]
    [SerializeField] private string dialogueID; 

    [Header("Diálogo")]
    [SerializeField] private string[] lines;
    [SerializeField] private bool mostrarUnaVez = true;

    public string DialogueID => dialogueID;
    public string[] Lines => lines;
    public bool MostrarUnaVez => mostrarUnaVez;
}