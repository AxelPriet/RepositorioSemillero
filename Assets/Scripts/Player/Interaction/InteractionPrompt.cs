using UnityEngine;

namespace EclipseGames.Player.Interaction
{
    public class InteractionPrompt : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Configuración")]
        [SerializeField] private float minOpacityDistance = 3f;  // Empieza a aparecer
        [SerializeField] private float maxOpacityDistance = 1f;  // Opacidad máxima
        [SerializeField] private AnimationCurve opacityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // Arriba del NPC

        private Transform targetTransform;
        private Transform playerTransform;

        private void Awake()
        {
            // Si no asignamos spriteRenderer, lo intentamos obtener
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            // Inicializar invisible
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1, 1, 1, 0);
        }

        private void Update()
        {
            if (targetTransform == null || playerTransform == null || spriteRenderer == null)
            {
                Destroy(gameObject);
                return;
            }

            // 1. Seguir al NPC
            transform.position = targetTransform.position + offset;

            // 2. Calcular distancia al jugador
            float distance = Vector3.Distance(playerTransform.position, targetTransform.position);

            // 3. Calcular opacidad
            float opacity = CalculateOpacity(distance);

            // 4. Aplicar opacidad al sprite
            Color color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;

            // 5. Escala dinámica (opcional)
            transform.localScale = Vector3.one * (0.8f + (opacity * 0.2f));
        }

        private float CalculateOpacity(float distance)
        {
            // Fuera del rango = invisible
            if (distance > minOpacityDistance)
                return 0f;

            // Muy cerca = opacidad máxima
            if (distance < maxOpacityDistance)
                return 1f;

            // Entre medias = interpolación suave
            float t = 1f - ((distance - maxOpacityDistance) / (minOpacityDistance - maxOpacityDistance));
            return opacityCurve.Evaluate(t);
        }

        public void SetTarget(Transform target)
        {
            targetTransform = target;
        }

        public void SetPlayer(Transform player)
        {
            playerTransform = player;
        }

        public void SetKeyText(string text)
        {
            // Para sprite, necesitaríamos cambiar el sprite según el texto
            // Por ahora solo E, lo dejamos así
        }
    }
}
