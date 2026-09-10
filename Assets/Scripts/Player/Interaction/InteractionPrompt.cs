using UnityEngine;

namespace EclipseGames.Player.Interaction
{
    public class InteractionPrompt : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Configuración")]
        [SerializeField] private float minOpacityDistance = 3f;
        [SerializeField] private float maxOpacityDistance = 1f;
        [SerializeField] private AnimationCurve opacityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

        [Header("Escala")]
        [SerializeField] private float escalaBase = 1f;          
        [SerializeField] private float escalaMinima = 0.8f;     
        [SerializeField] private float escalaMaxima = 1f;        

        private Transform targetTransform;
        private Transform playerTransform;
        private Vector3 escalaOriginal;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1, 1, 1, 0);

            escalaOriginal = transform.localScale;
        }

        private void Update()
        {
            if (targetTransform == null || playerTransform == null || spriteRenderer == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = targetTransform.position + offset;

            float distance = Vector3.Distance(playerTransform.position, targetTransform.position);
            float opacity = CalculateOpacity(distance);

            Color color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;

            float factorEscala = Mathf.Lerp(escalaMinima, escalaMaxima, opacity);
            transform.localScale = escalaOriginal * escalaBase * factorEscala;
        }

        private float CalculateOpacity(float distance)
        {
            if (distance > minOpacityDistance) return 0f;
            if (distance < maxOpacityDistance) return 1f;

            float t = 1f - ((distance - maxOpacityDistance) / (minOpacityDistance - maxOpacityDistance));
            return opacityCurve.Evaluate(t);
        }

        public void SetTarget(Transform target) => targetTransform = target;
        public void SetPlayer(Transform player) => playerTransform = player;
        public void SetKeyText(string text) { }
    }
}