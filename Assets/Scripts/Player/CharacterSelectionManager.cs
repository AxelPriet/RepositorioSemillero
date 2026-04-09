using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Control — desactiva esto mientras pruebas")]
    //[SerializeField] private bool activarSeleccion = false;

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


    private void Start()
    {

        if (PlayerData.Instance != null && PlayerData.Instance.PersonajeElegido)
        {
            gameObject.SetActive(false);
            return;
        }

        if (textoTitulo) textoTitulo.text = "Elige tu personaje";
        if (textoNicolas) textoNicolas.text = "Nicolás";
        if (textoMichell) textoMichell.text = "Michell";

        if (textoErrorNombre == null)
        {
            CrearTextoError();
        }
        else
        {
            textoErrorNombre.text = "";
        }

        botonConfirmarNombre.onClick.AddListener(ConfirmarNombre);
        botonNicolas.onClick.AddListener(() => SeleccionarPersonaje("Nicolas"));
        botonMichell.onClick.AddListener(() => SeleccionarPersonaje("Michell"));

        panelNombre.SetActive(true);
        panelPersonaje.SetActive(false);
    }

    private void CrearTextoError()
    {
        GameObject errorGo = new GameObject("TextoError");
        errorGo.transform.SetParent(panelNombre.transform, false);

        textoErrorNombre = errorGo.AddComponent<TextMeshProUGUI>();
        textoErrorNombre.text = "";
        textoErrorNombre.color = Color.red;
        textoErrorNombre.fontSize = 18;
        textoErrorNombre.alignment = TextAlignmentOptions.Center;

        RectTransform rect = textoErrorNombre.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -80);
        rect.sizeDelta = new Vector2(300, 40);
    }

    private void ConfirmarNombre()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            if (textoErrorNombre != null)
                textoErrorNombre.text = "Por favor ingresa tu nombre";
            return;
        }

        PlayerData.Instance?.SetNombre(nombre);

        panelNombre.SetActive(false);
        panelPersonaje.SetActive(true);
    }

    private void SeleccionarPersonaje(string personaje)
    {
        Debug.Log("Seleccionando personaje: " + personaje); // Para depurar

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.SetPersonaje(personaje);
        }
        else
        {
            Debug.LogError("PlayerData.Instance es nulo");
            return;
        }

        // Ocultar ambos paneles (nombre y personaje)
        if (panelNombre != null) panelNombre.SetActive(false);
        if (panelPersonaje != null) panelPersonaje.SetActive(false);

        // Opcional: desactivar todo el objeto del manager (si quieres que no vuelva a aparecer)
        // gameObject.SetActive(false); // Coméntalo si prefieres solo ocultar los paneles
    }
}