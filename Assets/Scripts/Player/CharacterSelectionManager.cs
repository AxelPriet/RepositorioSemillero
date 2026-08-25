using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPersonaje;

    [Header("Personajes")]
    [SerializeField] private Button botonNicolas;
    [SerializeField] private Button botonMichell;

    [Header("Escena del juego")]
    [SerializeField] private string escenaTutorial = "Tutorial";
    [SerializeField] private string escenaMain = "Main";

    private void Start()
    {
        botonNicolas.onClick.AddListener(() => SeleccionarPersonaje(0));
        botonMichell.onClick.AddListener(() => SeleccionarPersonaje(1));
        panelPersonaje.SetActive(false);
    }

    public void MostrarSeleccionPersonaje()
    {
        panelPersonaje.SetActive(true);
    }

    private void SeleccionarPersonaje(int personajeIndex)
    {
        PlayerData.Instance?.SetPersonajeIndex(personajeIndex);

        if (PlayerPrefs.GetInt("TutorialCompletado", 0) == 1)
            SceneManager.LoadScene(escenaMain);
        else
            SceneManager.LoadScene(escenaTutorial);
    }
}