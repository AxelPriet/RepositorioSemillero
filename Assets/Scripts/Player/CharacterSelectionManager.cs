using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Control — desactiva esto mientras pruebas")]
    [SerializeField] private bool activarSeleccion = false;

    [Header("Panel de nombre")]
    [SerializeField] private GameObject panelNombre;
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private Button botonConfirmarNombre;
    [SerializeField] private TextMeshProUGUI textoErrorNombre;

    [Header("Panel de selección de personaje")]
    [SerializeField] private GameObject panelPersonaje;
    [SerializeField] private TextMeshProUGUI textoTitulo;

    [Header("Personaje Masculino")]
    [SerializeField] private Image imagenNicolas;
    [SerializeField] private TextMeshProUGUI textoNicolas;
    [SerializeField] private Button botonNicolas;

    [Header("Personaje Femenino")]
    [SerializeField] private Image imagenMichell;
    [SerializeField] private TextMeshProUGUI textoMichell;
    [SerializeField] private Button botonMichell;

    [Header("Escena del juego")]
    [SerializeField] private int escenaJuego = 1;

    private void Start()
    {
        if (!activarSeleccion)
        {
            SceneManager.LoadScene(escenaJuego);
            return;
        }

        if (PlayerData.Instance != null && PlayerData.Instance.PersonajeElegido)
        {
            SceneManager.LoadScene(escenaJuego);
            return;
        }

        if (textoTitulo) textoTitulo.text = "Elige tu personaje";
        if (textoNicolas) textoNicolas.text = "Nicolás";
        if (textoMichell) textoMichell.text = "Michell";

        botonConfirmarNombre.onClick.AddListener(ConfirmarNombre);
        botonNicolas.onClick.AddListener(() => SeleccionarPersonaje("Nicolas"));
        botonMichell.onClick.AddListener(() => SeleccionarPersonaje("Michell"));

        panelNombre.SetActive(true);
        panelPersonaje.SetActive(false);
        if (textoErrorNombre) textoErrorNombre.text = "";
    }

    private void ConfirmarNombre()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            if (textoErrorNombre) textoErrorNombre.text = "Por favor ingresa tu nombre";
            return;
        }

        // Guardar nombre
        PlayerData.Instance?.SetNombre(nombre);

        // Pasar al panel de personaje
        panelNombre.SetActive(false);
        panelPersonaje.SetActive(true);
    }

    private void SeleccionarPersonaje(string personaje)
    {
        PlayerData.Instance?.SetPersonaje(personaje);
        SceneManager.LoadScene(escenaJuego);
    }
}