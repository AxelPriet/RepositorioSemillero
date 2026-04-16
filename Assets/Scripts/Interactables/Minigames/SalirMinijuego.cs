using UnityEngine;
using UnityEngine.SceneManagement;

public class SalirMinijuego : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    public void Salir()
    {
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }
}