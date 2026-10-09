using UnityEngine;

public class MobileControlsManager : MonoBehaviour
{
    public static MobileControlsManager Instance { get; private set; }

    [Header("Controles táctiles")]
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject botonInteractuar;
    [SerializeField] private GameObject botonMenu;
    [SerializeField] private GameObject botonAvanzarDialogo;

    [Header("Configuración")]
    [SerializeField] private bool soloEnMovil = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (botonAvanzarDialogo != null)
            botonAvanzarDialogo.SetActive(false);

        if (soloEnMovil && !Application.isMobilePlatform)
        {
            OcultarControlesTactiles();
            return;
        }

        MostrarControlesTactiles();
    }

    public void MostrarControlesTactiles()
    {
        if (joystick != null) joystick.SetActive(true);
        if (botonInteractuar != null) botonInteractuar.SetActive(true);
        if (botonMenu != null) botonMenu.SetActive(true);
    }

    public void OcultarControlesTactiles()
    {
        if (joystick != null) joystick.SetActive(false);
        if (botonInteractuar != null) botonInteractuar.SetActive(false);
        if (botonMenu != null) botonMenu.SetActive(false);
    }

    public void SetControlesActivos(bool activos)
    {
        if (soloEnMovil && !Application.isMobilePlatform) return;

        if (activos) MostrarControlesTactiles();
        else OcultarControlesTactiles();
    }

    public void MostrarBotonDialogo()
    {
        if (botonAvanzarDialogo != null)
            botonAvanzarDialogo.SetActive(true);
    }

    public void OcultarBotonDialogo()
    {
        if (botonAvanzarDialogo != null)
            botonAvanzarDialogo.SetActive(false);
    }
}