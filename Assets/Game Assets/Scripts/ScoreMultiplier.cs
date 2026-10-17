using TMPro;
using UnityEngine;

public class MultiplierDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text multiplierText;
    [SerializeField] string format = "{0}x";     // shows 1x, 2x, 4x...
    [SerializeField] int startMultiplier = 1;

    [Header("Color for 1x, 2x, 4x, 8x, 16x, 32x")]
    [SerializeField] Color[] tierColors =
    {
        new Color32(255, 255, 255, 255),   // 1x  white
        new Color32(183, 255, 160, 255),   // 2x  light green
        new Color32(225, 245, 110, 255),   // 4x  yellow-green
        new Color32(255, 190, 0, 255),     // 8x  amber
        new Color32(255, 120, 20, 255),    // 16x orange
        new Color32(255, 60, 60, 255),     // 32x red
    };

    // Other scripts can read this: score += points * MultiplierDisplay.Current
    public static int Current { get; private set; } = 1;

    void Start()
    {
        SetMultiplier(startMultiplier);
    }

    public void SetMultiplier(int m)
    {
        Current = Mathf.Max(1, m);
        multiplierText.text = string.Format(format, Current);

        int i = Mathf.Clamp(Mathf.RoundToInt(Mathf.Log(Current, 2)), 0, tierColors.Length - 1);
        multiplierText.color = tierColors[i];
    }

    [ContextMenu("Double multiplier (test)")]
    void DoubleTest() { SetMultiplier(Current >= 32 ? 1 : Current * 2); }
}