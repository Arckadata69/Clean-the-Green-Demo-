using TMPro;
using UnityEngine;

public class CapsDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text capsText;
    [SerializeField] string format = "({0})";   // shows (0), (25), (1,250)

    int last = -1;

    void Update()
    {
        int caps = PlayerData.Caps;
        if (caps == last) return;      // only redraw when the number changes
        last = caps;
        capsText.text = string.Format(format, caps.ToString("N0"));
    }

    [ContextMenu("Add 10 Caps (test)")]
    void AddTest() { PlayerData.Caps += 10; }
}