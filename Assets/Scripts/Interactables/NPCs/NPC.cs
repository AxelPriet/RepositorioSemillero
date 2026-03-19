using UnityEngine;

public class NPC : MonoBehaviour, IInteractuable
{
    [Header("Configuración del NPC")]
    [SerializeField] private string nombreNPC = "Profe Manuel";
    [SerializeField] private string[] lineasDialogo;
    [SerializeField] private bool puedeInteractuar = true;

    [Header("Feedback")]
    [SerializeField] private Color colorGizmo = Color.yellow;
    public void Interactuar()
    {
        if (!puedeInteractuar) return;

        if (lineasDialogo.Length > 0)
        {
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

    public void SetPuedeInteractuar(bool valor)
    {
        puedeInteractuar = valor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = colorGizmo;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}