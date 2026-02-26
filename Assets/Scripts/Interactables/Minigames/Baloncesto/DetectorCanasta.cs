using UnityEngine;

public class DetectorCanasta : MonoBehaviour
{
    private MinijuegoBaloncesto minijuego;

    public void SetMinijuego(MinijuegoBaloncesto minijuegoRef)
    {
        minijuego = minijuegoRef;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Balon"))
        {
            minijuego?.RegistrarCanasta();
        }
    }
}