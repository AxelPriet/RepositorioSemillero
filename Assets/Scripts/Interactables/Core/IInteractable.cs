using EclipseGames.Player.Interaction;
using UnityEngine;

public interface IInteractuable
{
    void Interactuar();  
    string GetPrompt();
    bool PuedeInteractuar();
    Transform GetTransform();
}