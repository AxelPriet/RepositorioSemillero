using UnityEngine;

public class CarnetManager : MonoBehaviour
{
    public static CarnetManager Instance;

    [SerializeField] private int totalPieces = 4;
    private int collectedPieces = 0;

    public int PartesRecolectadas => collectedPieces;
    public int TotalPieces => totalPieces;

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
    }
}