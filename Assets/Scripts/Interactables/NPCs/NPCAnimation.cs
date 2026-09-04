using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPCAnimation : MonoBehaviour
{
    [SerializeField] private NPCMovement movement;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (movement != null)
            movement.OnDirectionChanged += UpdateAnimation;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (animator == null) return;

        if (direction == Vector2.zero)
        {
            animator.SetBool("Moviendose", false);
            return;
        }

        animator.SetBool("Moviendose", true);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetInteger("Direccion", 2); // lateral
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x > 0;
        }
        else if (direction.y > 0)
        {
            animator.SetInteger("Direccion", 1); // arriba
        }
        else
        {
            animator.SetInteger("Direccion", 0); // abajo / frente
        }
    }
}