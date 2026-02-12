using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventarioJugador : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int capacidadMaxima = 20;

    [Header("Estado del Inventario")]
    [SerializeField] private List<Coleccionable> objetos = new List<Coleccionable>();
    [SerializeField] private Dictionary<string, int> contadorObjetos = new Dictionary<string, int>();

    // Singleton
    public static InventarioJugador Instance { get; private set; }

    private void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("<color=cyan>🎒 Sistema de inventario iniciado</color>");
        MostrarInventario();
    }

    // Añadir objeto al inventario
    public void AñadirObjeto(Coleccionable objeto)
    {
        if (objeto == null) return;

        // Verificar capacidad
        if (objetos.Count >= capacidadMaxima)
        {
            Debug.Log("<color=red>❌ Inventario lleno. No puedes recoger más objetos.</color>");
            return;
        }

        // Añadir a la lista
        objetos.Add(objeto);

        // Actualizar contador
        string id = objeto.GetID();
        if (contadorObjetos.ContainsKey(id))
        {
            contadorObjetos[id]++;
        }
        else
        {
            contadorObjetos[id] = 1;
        }

        // Feedback visual
        Debug.Log($"<color=green>✅ {objeto.GetNombre()} añadido al inventario</color>");
        MostrarInventario();
    }

    // Quitar objeto del inventario
    public bool QuitarObjeto(string idObjeto)
    {
        // Buscar el objeto
        Coleccionable objeto = objetos.Find(o => o.GetID() == idObjeto);

        if (objeto != null)
        {
            objetos.Remove(objeto);
            contadorObjetos[idObjeto]--;

            if (contadorObjetos[idObjeto] <= 0)
                contadorObjetos.Remove(idObjeto);

            Debug.Log($"<color=red>❌ {objeto.GetNombre()} eliminado del inventario</color>");
            MostrarInventario();
            return true;
        }

        return false;
    }

    // Verificar si tiene un objeto
    public bool TieneObjeto(string idObjeto)
    {
        return contadorObjetos.ContainsKey(idObjeto);
    }

    // Obtener cantidad de un objeto
    public int CantidadObjeto(string idObjeto)
    {
        return contadorObjetos.ContainsKey(idObjeto) ? contadorObjetos[idObjeto] : 0;
    }

    // Mostrar inventario en consola
    public void MostrarInventario()
    {
        Debug.Log("<color=white>═══════════════════════════════</color>");
        Debug.Log("<color=cyan>🎒 INVENTARIO DEL JUGADOR</color>");
        Debug.Log($"<color=white>Espacio: {objetos.Count}/{capacidadMaxima}</color>");

        if (objetos.Count == 0)
        {
            Debug.Log("<color=grey>📭 El inventario está vacío</color>");
        }
        else
        {
            foreach (var kvp in contadorObjetos)
            {
                // Buscar el nombre del objeto
                Coleccionable obj = objetos.Find(o => o.GetID() == kvp.Key);
                string nombre = obj != null ? obj.GetNombre() : kvp.Key;
                Debug.Log($"<color=yellow>📦 {nombre} x{kvp.Value}</color>");
            }
        }
        Debug.Log("<color=white>═══════════════════════════════</color>");
    }

    // Vaciar inventario
    public void VaciarInventario()
    {
        objetos.Clear();
        contadorObjetos.Clear();
        Debug.Log("<color=red>🧹 Inventario vaciado</color>");
    }
}