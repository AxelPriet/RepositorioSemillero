using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    [Header("Inventario UI")]
    [SerializeField] private TextMeshProUGUI totalColeccionablesText;
    [SerializeField] private TextMeshProUGUI partesCarnetText;

    private InputHandler inputHandler;
    private PlayerMovement playerMovement;

    private void OnEnable()
    {
        inputHandler = InputHandler.Instance;
        if (inputHandler != null)
            inputHandler.OnMenuToggle += ToggleMenu;

        if (CarnetManager.Instance != null)
            CarnetManager.Instance.OnPieceCollected += UpdateInventoryDisplay;
    }

    private void OnDisable()
    {
        if (inputHandler != null)
            inputHandler.OnMenuToggle -= ToggleMenu;

        if (CarnetManager.Instance != null)
            CarnetManager.Instance.OnPieceCollected -= UpdateInventoryDisplay;
    }

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (InputHandler.Instance != null && inputHandler == null)
        {
            inputHandler = InputHandler.Instance;
            inputHandler.OnMenuToggle += ToggleMenu;
        }
        if (gameObject.activeSelf)
            UpdateInventoryDisplay();
    }

    private void ToggleMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;

        if (mainMenu.activeSelf || optionsMenu.activeSelf)
        {
            if (optionsMenu.activeSelf) OpenMainMenuPanel();
            else ResumeGame();
        }
        else
        {
            OpenMenu();
            if (playerMovement != null)
                playerMovement.SetMovementEnabled(false);
        }
    }

    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        UpdateInventoryDisplay();
    }

    private void OpenMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        UpdateInventoryDisplay();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }

    public void ResumeGame()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void UpdateInventoryDisplay()
    {
        if (InventarioJugador.Instance != null)
        {
            int totalObjetos = InventarioJugador.Instance.ObjetosCount;
            if (totalColeccionablesText != null)
                totalColeccionablesText.text = totalObjetos.ToString();
        }

        if (CarnetManager.Instance != null)
        {
            int partes = CarnetManager.Instance.PartesRecolectadas;
            int total = CarnetManager.Instance.TotalPieces;
            if (partesCarnetText != null)
                partesCarnetText.text = partes + " / " + total;
        }
    }
}