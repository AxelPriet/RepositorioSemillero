using UnityEngine;
using System.Collections.Generic;

public class InventarioJugador : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int capacidadMaxima = 20;

    [Header("Estado del Inventario")]
    [SerializeField] private List<Coleccionable> objetos = new List<Coleccionable>();
    private Dictionary<string, int> contadorObjetos = new Dictionary<string, int>();

    public int ObjetosCount => objetos.Count;
    public int CapacidadMaxima => capacidadMaxima;

    public static InventarioJugador Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AñadirObjeto(Coleccionable objeto)
    {
        if (objeto == null) return;

        if (objetos.Count >= capacidadMaxima) return;

        objetos.Add(objeto);

        string id = objeto.GetID();
        if (contadorObjetos.ContainsKey(id))
            contadorObjetos[id]++;
        else
            contadorObjetos[id] = 1;
    }

    public bool QuitarObjeto(string idObjeto)
    {
        Coleccionable objeto = objetos.Find(o => o.GetID() == idObjeto);
        if (objeto != null)
        {
            objetos.Remove(objeto);
            contadorObjetos[idObjeto]--;

            if (contadorObjetos[idObjeto] <= 0)
                contadorObjetos.Remove(idObjeto);

            return true;
        }
        return false;
    }

    public bool TieneObjeto(string idObjeto)
    {
        return contadorObjetos.ContainsKey(idObjeto);
    }

    public int CantidadObjeto(string idObjeto)
    {
        return contadorObjetos.ContainsKey(idObjeto) ? contadorObjetos[idObjeto] : 0;
    }

    public void VaciarInventario()
    {
        objetos.Clear();
        contadorObjetos.Clear();
    }
}