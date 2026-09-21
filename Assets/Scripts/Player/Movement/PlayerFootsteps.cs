using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private bool estaMoviendose = false;

    private void Update()
    {
        estaMoviendose = InputHandler.Instance != null &&
                         InputHandler.Instance.GetMoveInput() != Vector2.zero;
    }

    public void PasoIzquierdo()
    {
        if (!estaMoviendose) return;
        AudioManager.Instance?.PlaySFX("sfx_footstep_left");
    }

    public void PasoDerecho()
    {
        if (!estaMoviendose) return;
        AudioManager.Instance?.PlaySFX("sfx_footstep_right");
    }
}