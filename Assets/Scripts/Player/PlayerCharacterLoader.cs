using UnityEngine;

public class PlayerCharacterLoader : MonoBehaviour
{
    [Header("Animator del jugador")]
    [SerializeField] private Animator playerAnimator;

    [Header("Override Controllers (mismo orden que en selección)")]
    [SerializeField] private AnimatorOverrideController[] characterControllers; // [0] Nicolás, [1] Michell

    private void Start()
    {
        if (PlayerData.Instance != null && PlayerData.Instance.PersonajeElegido)
        {
            int index = PlayerData.Instance.PersonajeIndex;

            if (characterControllers != null && characterControllers.Length > index && characterControllers[index] != null)
            {
                playerAnimator.runtimeAnimatorController = characterControllers[index];
            }
        }
    }
}
