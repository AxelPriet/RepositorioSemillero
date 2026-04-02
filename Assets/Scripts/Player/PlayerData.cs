using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public string NombreJugador { get; private set; } = "";
    public string PersonajeSeleccionado { get; private set; } = "";
    public bool PersonajeElegido => !string.IsNullOrEmpty(PersonajeSeleccionado);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetNombre(string nombre)
    {
        NombreJugador = nombre;
        Debug.Log($"[PlayerData] Nombre: {nombre}");
    }

    public void SetPersonaje(string personaje)
    {
        PersonajeSeleccionado = personaje;
        Debug.Log($"[PlayerData] Personaje: {personaje}");
    }

    public void Reset()
    {
        NombreJugador = "";
        PersonajeSeleccionado = "";
    }
}