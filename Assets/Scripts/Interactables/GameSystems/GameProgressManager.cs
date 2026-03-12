using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [SerializeField] private int totalMinijuegos = 18; 
    private bool[] minijuegosCompletados;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarProgreso();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarProgreso()
    {
        minijuegosCompletados = new bool[totalMinijuegos];

        for (int i = 0; i < totalMinijuegos; i++)
        {
            minijuegosCompletados[i] = PlayerPrefs.GetInt($"Minijuego_{i}", 0) == 1;
        }
    }

    public bool PuedeJugar(int index)
    {
        return !minijuegosCompletados[index];
    }

    public void CompletarMinijuego(int index)
    {
        if (!minijuegosCompletados[index])
        {
            minijuegosCompletados[index] = true;
            PlayerPrefs.SetInt($"Minijuego_{index}", 1);
            PlayerPrefs.Save();

            CarnetManager.Instance?.AddCarnetPiece();

            Debug.Log($"Minijuego {index} completado. Progreso guardado.");
        }
    }

    public bool EstaCompletado(int index)
    {
        return minijuegosCompletados[index];
    }

    public int ObtenerProgreso()
    {
        int completados = 0;
        foreach (bool completado in minijuegosCompletados)
        {
            if (completado) completados++;
        }
        return completados;
    }
}