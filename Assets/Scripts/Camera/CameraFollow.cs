using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform target; 

    [Header("Configuración de Seguimiento")]
    [SerializeField] private float smoothTime = 0.2f; 
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); 

    [Header("Límites de Cámara (Opcional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    
    private Vector3 velocity = Vector3.zero;

    
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        
        Vector3 targetPosition = target.position + offset;

        
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }

    
    public void DisableBounds()
    {
        useBounds = false;
    }

    
    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(
                (minBounds.x + maxBounds.x) / 2,
                (minBounds.y + maxBounds.y) / 2,
                0
            ), new Vector3(
                maxBounds.x - minBounds.x,
                maxBounds.y - minBounds.y,
                0
            ));
        }
    }
}
