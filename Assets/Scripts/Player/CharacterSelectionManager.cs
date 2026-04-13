using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelNombre;
    [SerializeField] private GameObject panelPersonaje;

    [Header("Nombre")]
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private Button botonConfirmarNombre;

    [Header("Personajes")]
    [SerializeField] private Button botonNicolas;
    [SerializeField] private Button botonMichell;

    [Header("Escena del juego")]
    [SerializeField] private int escenaJuego = 1;

    private void Start()
    {
        botonConfirmarNombre.onClick.AddListener(ConfirmarNombre);
        botonNicolas.onClick.AddListener(() => SeleccionarPersonaje(0));
        botonMichell.onClick.AddListener(() => SeleccionarPersonaje(1));

        if (PlayerData.Instance != null && PlayerData.Instance.PersonajeElegido)
        {
            panelNombre.SetActive(false);
            panelPersonaje.SetActive(false);
            return;
        }

        panelNombre.SetActive(false);
        panelPersonaje.SetActive(false);
    }

    public void MostrarPanelNombre()
    {
        if (PlayerData.Instance != null && PlayerData.Instance.PersonajeElegido)
        {
            SceneManager.LoadScene(escenaJuego);
            return;
        }

        panelNombre.SetActive(true);
        panelPersonaje.SetActive(false);
        if (inputNombre != null) inputNombre.text = "";
    }


    private void ConfirmarNombre()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
            return;

        PlayerData.Instance?.SetNombre(nombre);

        panelNombre.SetActive(false);
        panelPersonaje.SetActive(true);
    }

    private void SeleccionarPersonaje(int personajeIndex)
    {
        PlayerData.Instance?.SetPersonajeIndex(personajeIndex);
        SceneManager.LoadScene(escenaJuego);
    }
}