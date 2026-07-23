using UnityEngine;

[CreateAssetMenu(fileName = "NuevoDialogoTutorial", menuName = "Tutorial/Dialogo Tutorial")]
public class TutorialDialogoSO : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lineas;
}