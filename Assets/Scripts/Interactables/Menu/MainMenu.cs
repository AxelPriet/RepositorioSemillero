using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    [Header("Selección de Personaje")]
    [SerializeField] private CharacterSelectionManager characterSelectionManager;

    [Header("Inventario UI")]
    [SerializeField] private TextMeshProUGUI totalColeccionablesText;
    [SerializeField] private TextMeshProUGUI partesCarnetText;
    [SerializeField] private GameObject inventarioPanel; 

    private InputHandler inputHandler;
    private PlayerMovement playerMovement;
    public bool EstaAbierto => mainMenu.activeSelf || optionsMenu.activeSelf;

    private void OnEnable()
    {
        inputHandler = InputHandler.Instance;
        if (inputHandler != null)
            inputHandler.OnMenuToggle += ToggleMenu;

        if (CarnetManager.Instance != null)
            CarnetManager.Instance.OnPieceCollected += UpdateInventoryDisplay;

        if (InventarioJugador.Instance != null)
            InventarioJugador.Instance.OnObjetoRecolectado += UpdateInventoryDisplay;
    }

    private void OnDisable()
    {
        if (inputHandler != null)
            inputHandler.OnMenuToggle -= ToggleMenu;

        if (CarnetManager.Instance != null)
            CarnetManager.Instance.OnPieceCollected -= UpdateInventoryDisplay;

        if (InventarioJugador.Instance != null)
            InventarioJugador.Instance.OnObjetoRecolectado -= UpdateInventoryDisplay;
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

        if (inventarioPanel != null)
            inventarioPanel.SetActive(false);
    }

    public void PlayGame()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);

        if (PlayerData.Instance != null && PlayerData.Instance.PersonajeElegido)
        {
            SceneManager.LoadScene("Main");
            return;
        }

        if (characterSelectionManager != null)
            characterSelectionManager.MostrarSeleccionPersonaje();
    }

    public void ResumeGame()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        if (inventarioPanel != null)
            inventarioPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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