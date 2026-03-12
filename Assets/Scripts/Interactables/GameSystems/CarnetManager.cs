using UnityEngine;
using TMPro;

public class CarnetManager : MonoBehaviour
{
    public static CarnetManager Instance;

    [Header("Configuración")]
    [SerializeField] private int totalPieces = 18; 
    [SerializeField] private TextMeshProUGUI textoContador; 

    private int collectedPieces = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CargarProgreso();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CargarProgreso()
    {
        collectedPieces = PlayerPrefs.GetInt("CarnetPieces", 0);
        ActualizarUI();
    }

    public void AddCarnetPiece()
    {
        collectedPieces++;
        PlayerPrefs.SetInt("CarnetPieces", collectedPieces);
        PlayerPrefs.Save();

        ActualizarUI();
        Debug.Log($"Parte del carnet obtenida: {collectedPieces}/{totalPieces}");

        if (collectedPieces >= totalPieces)
        {
            Debug.Log("¡Carnet completado!");
        }
    }

    public int GetCollectedPieces()
    {
        return collectedPieces;
    }

    public int GetTotalPieces()
    {
        return totalPieces;
    }

    private void ActualizarUI()
    {
        if (textoContador != null)
            textoContador.text = $"Carnet: {collectedPieces}/{totalPieces}";
    }
    public void ReiniciarProgreso()
    {
        collectedPieces = 0;
        PlayerPrefs.SetInt("CarnetPieces", 0);
        PlayerPrefs.Save();
        ActualizarUI();
        Debug.Log("Progreso del carnet reiniciado");
    }
}