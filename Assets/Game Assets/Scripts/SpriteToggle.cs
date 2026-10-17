using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SpriteToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("Save key (must be unique per toggle)")]
    [SerializeField] string saveKey = "music";

    [Header("Tile (this object)")]
    [SerializeField] Sprite activeSprite;     // green "Active"
    [SerializeField] Sprite inactiveSprite;   // red "Inactive"

    [Header("Icon (optional)")]
    [SerializeField] SpriteRenderer iconRenderer;
    [SerializeField] Sprite iconOn;           // e.g. Volume (On)
    [SerializeField] Sprite iconOff;          // e.g. Volume (Off)

    [Header("Hook up your game logic here")]
    public UnityEvent<bool> onChanged;

    SpriteRenderer tile;
    public bool IsOn { get; private set; }

    void Awake()
    {
        tile = GetComponent<SpriteRenderer>();
        IsOn = PlayerPrefs.GetInt("toggle_" + saveKey, 1) == 1;   // default: on
        Apply();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsOn = !IsOn;
        PlayerPrefs.SetInt("toggle_" + saveKey, IsOn ? 1 : 0);
        Apply();
        onChanged.Invoke(IsOn);
    }

    void Apply()
{
    tile.sprite = IsOn ? activeSprite : inactiveSprite;

    if (iconRenderer != null)
    {
        Sprite s = IsOn ? iconOn : iconOff;
        if (s != null) iconRenderer.sprite = s;
        else Debug.LogWarning(name + ": icon sprite slot is empty", this);
    }
}
}