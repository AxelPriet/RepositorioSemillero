using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    [Header("Sonidos")]
    [SerializeField] private string hoverSoundID = "sfx_ui_hover";
    [SerializeField] private string clickSoundID = "sfx_ui_click";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instance?.PlayUI(clickSoundID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable)
            AudioManager.Instance?.PlayUI(hoverSoundID);
    }
}