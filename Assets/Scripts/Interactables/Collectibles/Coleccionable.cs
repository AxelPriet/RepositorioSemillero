using UnityEngine;

public class Coleccionable : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Objeto")]
    [SerializeField] private string idObjeto = "objeto_1";
    [SerializeField] private string nombreObjeto = "Objeto Misterioso";
    [SerializeField] private bool puedeInteractuar = true;

    [Header("Comportamiento")]
    [SerializeField] private bool destruirAlRecoger = true;

    // Implementación de IInteractuable
    public void Interactuar()
    {
        if (!puedeInteractuar) return;

        // 1. Añadir al inventario del jugador
        InventarioJugador.Instance.AñadirObjeto(this);

        // 2. Feedback en consola
        Debug.Log($"<color=yellow>¡Has recogido: {nombreObjeto}!</color>");

        // 3. Desactivar interacción
        puedeInteractuar = false;

        // 4. Destruir o desactivar el objeto
        if (destruirAlRecoger)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public string GetPrompt()
    {
        return $"Recoger {nombreObjeto}";
    }

    public bool PuedeInteractuar()
    {
        return puedeInteractuar;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // Getters para el inventario
    public string GetID() => idObjeto;
    public string GetNombre() => nombreObjeto;
}