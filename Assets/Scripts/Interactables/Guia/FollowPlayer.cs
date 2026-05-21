using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private Animator playerAnimator;
    private Animator guideAnimator;
    private SpriteRenderer guideSpriteRenderer;
    private SpriteRenderer playerSpriteRenderer;

    [Header("Levitación")]
    [SerializeField] public float amplitudLevitacion = 0.08f;
    [SerializeField] public float velocidadLevitacion = 5f;
    private Vector3 offsetBase;

    [SerializeField] private Vector3 offset = new Vector3(1.5f, 0.5f, 0f);
    [SerializeField] private float smoothSpeed = 5f;

    private static readonly int MovementHash = Animator.StringToHash("Movement");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private void Start()
    {
        BuscarPlayer();
        offsetBase = offset;
    }

    private void BuscarPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerAnimator = p.GetComponent<Animator>();
            playerSpriteRenderer = p.GetComponent<SpriteRenderer>();
            guideAnimator = GetComponent<Animator>();
            guideSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            BuscarPlayer();
            return;
        }

        float levitacion = Mathf.Sin(Time.time * velocidadLevitacion) * amplitudLevitacion;
        Vector3 offsetConLevitacion = offsetBase + new Vector3(0f, levitacion, 0f);
        Vector3 targetPosition = player.position + offsetConLevitacion;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        if (playerAnimator == null || guideAnimator == null) return;

        float movement = playerAnimator.GetFloat(MovementHash);
        float moveX = playerAnimator.GetFloat(MoveXHash);
        float moveY = playerAnimator.GetFloat(MoveYHash);

        guideAnimator.SetFloat(MovementHash, movement);

        // Idle — reproducir Down
        if (movement < 0.1f)
        {
            guideAnimator.SetFloat(MoveXHash, 0f);
            guideAnimator.SetFloat(MoveYHash, -1f);
            Debug.Log($"IDLE - MoveX: {guideAnimator.GetFloat(MoveXHash)} | MoveY: {guideAnimator.GetFloat(MoveYHash)} | Estado actual: {guideAnimator.GetCurrentAnimatorStateInfo(0).IsName("GuiaDown")}");
            return;
        }

        // Diagonal o solo X — priorizar Side
        if (Mathf.Abs(moveX) > 0f)
        {
            guideAnimator.SetFloat(MoveXHash, moveX > 0 ? 1f : -1f);
            guideAnimator.SetFloat(MoveYHash, 0f);
        }
        else
        {
            // Solo vertical (W o S)
            guideAnimator.SetFloat(MoveXHash, 0f);
            guideAnimator.SetFloat(MoveYHash, moveY > 0 ? 1f : -1f); // W = Up, S = Down
        }

        if (guideSpriteRenderer != null && playerSpriteRenderer != null)
            guideSpriteRenderer.flipX = playerSpriteRenderer.flipX;
    }
}