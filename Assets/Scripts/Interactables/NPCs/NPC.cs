using UnityEngine;

public class NPC : MonoBehaviour, IInteractuable
{
    [Header("Configuración del NPC")]
    [SerializeField] private string nombreNPC = "Aldeano";
    [SerializeField] private string[] lineasDialogo;
    [SerializeField] private bool puedeInteractuar = true;

    [Header("Feedback")]
    [SerializeField] private Color colorGizmo = Color.yellow;

    // Implementación de IInteractuable
    public void Interactuar()
    {
        if (!puedeInteractuar) return;

        Debug.Log($"<color=green>{nombreNPC}: ¡HOLA! Has interactuado conmigo</color>");

        if (lineasDialogo.Length > 0)
        {
            Debug.Log($"<color=white>{nombreNPC}: {lineasDialogo[0]}</color>");
        }
    }

    public string GetPrompt()
    {
        return $"Hablar con {nombreNPC}";
    }

    public bool PuedeInteractuar()
    {
        return puedeInteractuar;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // Método para activar/desactivar
    public void SetPuedeInteractuar(bool valor)
    {
        puedeInteractuar = valor;
    }

    // Visualizar área de interacción
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = colorGizmo;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}