using UnityEngine;
using UnityEngine.EventSystems;

public class MarcoArrastrable : MonoBehaviour, IDragHandler
{
    private bool puedeMoverse = false;

    public void SetPuedeMoverse(bool estado)
    {
        puedeMoverse = estado;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (puedeMoverse)
        {
            transform.position += (Vector3)eventData.delta;
        }
    }
}
