using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] SpriteRenderer baseLayer;
    [SerializeField] SpriteRenderer fadeLayer;
    [SerializeField] Sprite[] stages;            // Dawn, Day, Dusk, Night
    [SerializeField] float fadeSeconds = 5f;

    Vector2 targetSize;

    void Awake()
    {
        targetSize = baseLayer.bounds.size;   // world size of the sky you set up
    }

    void Update()
    {
        int n = stages.Length;
        float t = GameClock.Elapsed / GameClock.SecondsPerStage;
        int whole = Mathf.FloorToInt(t);
        float frac = t - whole;
        int i = whole % n;

        float fadeStart = 1f - fadeSeconds / GameClock.SecondsPerStage;
        float alpha = frac > fadeStart
            ? Mathf.SmoothStep(0f, 1f, (frac - fadeStart) / (1f - fadeStart))
            : 0f;

        SetSprite(baseLayer, stages[i]);
        SetSprite(fadeLayer, stages[(i + 1) % n]);

        var c = fadeLayer.color;
        c.a = alpha;
        fadeLayer.color = c;
    }

    void SetSprite(SpriteRenderer r, Sprite s)
    {
        if (s == null) return;
        r.sprite = s;

        Vector3 parent = r.transform.parent ? r.transform.parent.lossyScale : Vector3.one;
        r.transform.localScale = new Vector3(
            targetSize.x / s.bounds.size.x / parent.x,
            targetSize.y / s.bounds.size.y / parent.y,
            1f);
    }
}