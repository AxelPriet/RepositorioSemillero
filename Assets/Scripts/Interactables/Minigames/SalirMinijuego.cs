using UnityEngine;

public class SalirMinijuego : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    public void Salir()
    {
        TransicionEscenas.Instance.CargarEscena(nombreEscenaPrincipal);
    }
}