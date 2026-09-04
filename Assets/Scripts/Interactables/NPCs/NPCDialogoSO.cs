using UnityEngine;

[CreateAssetMenu(fileName = "DialogoNPC", menuName = "NPC/Nuevo Dialogo", order = 1)]
public class NPCDialogoSO : ScriptableObject
{
    [Header("Información del NPC")]
    [SerializeField] private string npcName = "NPC";

    [Header("Líneas de diálogo")]
    [SerializeField] private string[] lineas;
    [SerializeField] private bool aleatorio = false;

    public string NPCName => npcName;
    public string[] Lineas => lineas;
    public bool Aleatorio => aleatorio;

    public string ObtenerLinea()
    {
        if (lineas == null || lineas.Length == 0) return "...";
        return aleatorio ? lineas[Random.Range(0, lineas.Length)] : lineas[0];
    }
}