using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private Animator playerAnimator;
    private Animator guideAnimator;
    private SpriteRenderer guideSpriteRenderer;

    [SerializeField] private Vector3 offset = new Vector3(1.5f, 0.5f, 0f);
    [SerializeField] private float smoothSpeed = 5f;

    private void Start()
    {
        BuscarPlayer();
    }

    private void BuscarPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerAnimator = p.GetComponent<Animator>();
            guideAnimator = GetComponent<Animator>();
            guideSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            BuscarPlayer();

        if (player == null) return;

        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        if (playerAnimator != null && guideAnimator != null)
        {
            CopyAnimatorParameter("Movement");
            CopyAnimatorParameter("MoveX");
            CopyAnimatorParameter("MoveY");
        }

        if (guideSpriteRenderer != null && player.GetComponent<SpriteRenderer>() != null)
        {
            guideSpriteRenderer.flipX = player.GetComponent<SpriteRenderer>().flipX;
        }
    }

    private void CopyAnimatorParameter(string parameterName)
    {
        var paramType = playerAnimator.GetFloat(parameterName);
        guideAnimator.SetFloat(parameterName, paramType);
    }
}