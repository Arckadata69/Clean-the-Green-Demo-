using System.Collections;
using UnityEngine;

public class MapSlider : MonoBehaviour
{
    [SerializeField] Transform[] maps;       // Map 1, Map 2, Map 3 (in order)
    [SerializeField] float spacing = 0f;     // 0 = auto (one map width)
    [SerializeField] float duration = 0.4f;
    [SerializeField] int startIndex = 0;

    [Header("Optional: hide arrows at the ends")]
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;

    int index;
    float centerX;
    Coroutine routine;

    // Read by TrashSpawner
    public int Index => index;
    public Transform CurrentMap => maps[index];
    public bool IsSliding { get; private set; }

    void Awake()
    {
        index = startIndex;
        centerX = maps[startIndex].localPosition.x;
        if (spacing <= 0f) spacing = MapWidth(maps[0]);

        for (int i = 0; i < maps.Length; i++)
            SetX(maps[i], TargetX(i));
        UpdateArrows();
    }

    public void Next()     => Go(+1);   // maps slide left
    public void Previous() => Go(-1);   // maps slide right

    void Go(int dir)
    {
        int n = Mathf.Clamp(index + dir, 0, maps.Length - 1);
        if (n == index) return;
        index = n;
        UpdateArrows();
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Slide());
    }

    IEnumerator Slide()
    {
        IsSliding = true;

        float[] from = new float[maps.Length];
        for (int i = 0; i < maps.Length; i++) from[i] = maps[i].localPosition.x;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float e = Mathf.SmoothStep(0f, 1f, t);
            for (int i = 0; i < maps.Length; i++)
                SetX(maps[i], Mathf.Lerp(from[i], TargetX(i), e));
            yield return null;
        }
        for (int i = 0; i < maps.Length; i++) SetX(maps[i], TargetX(i));

        IsSliding = false;
    }

    float TargetX(int i) => centerX + (i - index) * spacing;

    void SetX(Transform t, float x)
    {
        var p = t.localPosition;
        t.localPosition = new Vector3(x, p.y, p.z);
    }

    void UpdateArrows()
    {
        if (leftButton)  leftButton.SetActive(index > 0);
        if (rightButton) rightButton.SetActive(index < maps.Length - 1);
    }

    float MapWidth(Transform t)
    {
        var rt = t as RectTransform;
        if (rt != null) return rt.rect.width * rt.localScale.x;
        var sr = t.GetComponent<SpriteRenderer>();
        if (sr != null) return sr.sprite.bounds.size.x * t.localScale.x;
        return 1080f;
    }
}