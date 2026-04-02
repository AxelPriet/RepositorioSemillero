using UnityEngine;

public class Coleccionable : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Objeto")]
    [SerializeField] private string idObjeto = "objeto_1";
    [SerializeField] private string nombreObjeto = "Objeto Misterioso";
    [SerializeField] private bool puedeInteractuar = true;

    [Header("Comportamiento")]
    [SerializeField] private bool destruirAlRecoger = true;

    private void Start()
    {
        if (InventarioJugador.Instance != null &&
            InventarioJugador.Instance.YaFueRecogido(idObjeto))
        {
            gameObject.SetActive(false);
        }
    }

    public void Interactuar()
    {
        if (!puedeInteractuar) return;

        InventarioJugador.Instance?.AñadirObjeto(this);
        puedeInteractuar = false;

        if (destruirAlRecoger)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    public string GetPrompt() => $"Recoger {nombreObjeto}";
    public bool PuedeInteractuar() => puedeInteractuar;
    public Transform GetTransform() => transform;
    public string GetID() => idObjeto;
    public string GetNombre() => nombreObjeto;
}