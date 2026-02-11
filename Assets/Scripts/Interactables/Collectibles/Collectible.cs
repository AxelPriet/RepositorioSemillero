using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    [SerializeField] private string itemID = "easter_egg_1";
    [SerializeField] private string itemName = "Huevo de Pascua Secreto";
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private string description = "Un objeto misterioso...";

    [Header("Comportamiento")]
    [SerializeField] private bool destroyOnCollect = true;
    [SerializeField] private float collectionCooldown = 0.5f;

    [Header("Eventos")]
    public UnityEvent OnCollected;

    private bool isCollected = false;
    private float lastCollectionTime = -999f;

    public void Interact(PlayerInteraction player)
    {
        if (!CanInteract()) return;

        lastCollectionTime = Time.time;
        isCollected = true;

        Debug.Log($"🥚 ¡Coleccionable encontrado: {itemName}! ({itemID})");

        // Aquí añadirías al inventario
        // InventorySystem.AddItem(itemID, 1);

        // Evento para logros/sistema de easter eggs
        //EasterEggEvents.OnEggFound?.Invoke(itemID);
        OnCollected?.Invoke();

        // Feedback visual/sonoro
        // GetComponent<Animator>()?.SetTrigger("Collect");
        // GetComponent<AudioSource>()?.PlayOneShot(collectSound);

        if (destroyOnCollect)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    public string GetInteractionPrompt()
    {
        if (isCollected) return "";
        return $"Recoger {itemName} (E)";
    }

    public bool CanInteract()
    {
        if (isCollected) return false;
        if (Time.time - lastCollectionTime < collectionCooldown) return false;
        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}