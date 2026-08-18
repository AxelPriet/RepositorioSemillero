using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private string itemIDEsperado;
    [SerializeField] private MinijuegoMochila minijuego;

    private bool ocupada = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (ocupada) return;

        DraggableItem item = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (item == null) return;

        if (item.itemID == itemIDEsperado)
        {
            ocupada = true;
            item.ColocarEnSilueta(transform);
            minijuego.RegistrarObjetoColocado();
        }
    }
}