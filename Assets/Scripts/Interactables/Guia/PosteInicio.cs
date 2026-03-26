using UnityEngine;
using EclipseGames.Player.Interaction;

public class PosteInicio : MonoBehaviour, IInteractuable
{
    private bool interactuado = false;

    public void Interactuar()
    {
        if (interactuado) return;
        interactuado = true;

        GuideManager.Instance?.TriggerEvent(GuideManager.GuideEvent.BienvenidaInicio);
    }

    public string GetPrompt() => "E";
    public bool PuedeInteractuar() => !interactuado;
    public Transform GetTransform() => transform;
}