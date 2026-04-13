using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public string NombreJugador { get; private set; } = "";
    private int personajeIndex = -1;

    public int PersonajeIndex => personajeIndex;
    public bool PersonajeElegido => personajeIndex >= 0; 

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
    }

    public void SetPersonajeIndex(int index)
    {
        personajeIndex = index;
    }

    public void Reset()
    {
        NombreJugador = "";
        personajeIndex = 0;
    }
}