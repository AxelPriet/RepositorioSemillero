using UnityEngine;

public class CarnetManager : MonoBehaviour
{
    public static CarnetManager Instance;

    [SerializeField] private int totalPieces = 4;
    private int collectedPieces = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCarnetPiece()
    {
        collectedPieces++;

        Debug.Log("Parte del carnet obtenida: " + collectedPieces);

        if (collectedPieces >= totalPieces)
        {
            Debug.Log("Carnet completado. Juego terminado.");
        }
    }
}
