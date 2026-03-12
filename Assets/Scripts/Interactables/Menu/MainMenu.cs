using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject menuOptions;

    [Header("Indicador Carnet")]
    [SerializeField] private TextMeshProUGUI textoProgresoCarnet;

    [Header("Opciones")]
    [SerializeField] private Toggle toggleFullScreen;
    [SerializeField] private Slider sliderVolumen;

    private bool menuActivo = false;

    private void Awake()
    {
        mainMenu?.SetActive(false);
        menuOptions?.SetActive(false);

        CargarOpciones();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }

        if (menuActivo && textoProgresoCarnet != null && CarnetManager.Instance != null)
        {
            int actual = CarnetManager.Instance.GetCollectedPieces();
            int total = CarnetManager.Instance.GetTotalPieces();
            textoProgresoCarnet.text = $"Carnet: {actual}/{total}";
        }
    }

    public void ToggleMenu()
    {
        menuActivo = !menuActivo;

        if (menuActivo)
            AbrirMenu();
        else
            CerrarMenu();
    }

    private void AbrirMenu()
    {
        Time.timeScale = 0f;
        mainMenu?.SetActive(true);
        menuOptions?.SetActive(false);
    }

    private void CerrarMenu()
    {
        Time.timeScale = 1f;
        mainMenu?.SetActive(false);
        menuOptions?.SetActive(false);
    }

    // ===== MÉTODOS PARA BOTONES =====

    public void OnClickPlay()
    {
        ToggleMenu();
    }

    public void OnClickOptions()
    {
        mainMenu?.SetActive(false);
        menuOptions?.SetActive(true);
    }

    public void OnClickBack()
    {
        menuOptions?.SetActive(false);
        mainMenu?.SetActive(true);
        GuardarOpciones();
    }

    public void OnClickExit()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void OnToggleFullScreen(bool valor)
    {
        Screen.fullScreen = valor;
    }

    public void OnSliderVolumen(float valor)
    {
        AudioListener.volume = valor;
    }

    private void CargarOpciones()
    {
        if (toggleFullScreen != null)
            toggleFullScreen.isOn = PlayerPrefs.GetInt("FullScreen", 1) == 1;

        if (sliderVolumen != null)
            sliderVolumen.value = PlayerPrefs.GetFloat("Volumen", 1f);
    }

    private void GuardarOpciones()
    {
        if (toggleFullScreen != null)
            PlayerPrefs.SetInt("FullScreen", toggleFullScreen.isOn ? 1 : 0);

        if (sliderVolumen != null)
            PlayerPrefs.SetFloat("Volumen", sliderVolumen.value);

        PlayerPrefs.Save();
    }
}