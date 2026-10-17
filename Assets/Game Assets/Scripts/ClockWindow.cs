using TMPro;
using UnityEngine;

public class ClockDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text dayText;
    [SerializeField] TMP_Text timeText;

    [Header("Labels")]
    [SerializeField] string dayLabel = "Day: ";
    [SerializeField] string timeLabel = "Time: ";

    [SerializeField] bool followSettingsToggle = false;  // tick to obey the 24-hour toggle

    int lastHour = -1;

    void Update()
    {
        int h = Mathf.FloorToInt(GameClock.Hour24) % 24;
        if (h == lastHour) return;
        lastHour = h;

        bool use24 = followSettingsToggle && PlayerPrefs.GetInt("toggle_time24", 1) == 1;
        string time;
        if (use24)
        {
            time = $"{h:00}:00";
        }
        else
        {
            int h12 = h % 12;
            if (h12 == 0) h12 = 12;
            time = $"{h12:00}:00 {(h < 12 ? "am" : "pm")}";
        }

        timeText.text = timeLabel + time;
        dayText.text = dayLabel + GameClock.Day;
    }
}