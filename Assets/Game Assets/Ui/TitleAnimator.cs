using System.Collections;
using UnityEngine;

public class TitleAnimator : MonoBehaviour
{
    [Header("Drop-in")]
    [Tooltip("How far above its final spot the title starts (UI pixels or world units)")]
    [SerializeField] private float startOffsetY = 600f;
    [SerializeField] private float startDelay = 0.3f;
    [SerializeField] private float fallDuration = 2f;
    [Tooltip("Ease-out = fast then settles slowly. Edit the curve for a bounce.")]
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Idle bob")]
    [SerializeField] private float bobAmount = 1f;   // how far up and down
    [SerializeField] private float bobSpeed = 1.5f;   // higher = faster

    [Header("Options")]
    [SerializeField] private bool useUnscaledTime = true; // keeps working if timeScale = 0

    private RectTransform rect;
    private Vector3 finalPos;

    private void Awake()
    {
        rect = transform as RectTransform;
        finalPos = GetPos();
    }

    private void OnEnable()
    {
        // Move to the start position right away so it doesn't flash at the end spot
        SetPos(finalPos + Vector3.up * startOffsetY);
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        if (startDelay > 0f)
            yield return Wait(startDelay);

        // 1) Fall down slowly
        Vector3 startPos = finalPos + Vector3.up * startOffsetY;
        float t = 0f;
        while (t < 1f)
        {
            t += Delta() / Mathf.Max(fallDuration, 0.01f);
            SetPos(Vector3.LerpUnclamped(startPos, finalPos, fallCurve.Evaluate(Mathf.Clamp01(t))));
            yield return null;
        }
        SetPos(finalPos);

        // 2) Idle up and down forever (starts at the final position, so no jump)
        float time = 0f;
        while (true)
        {
            time += Delta();
            float y = Mathf.Sin(time * bobSpeed) * bobAmount;
            SetPos(finalPos + Vector3.up * y);
            yield return null;
        }
    }

    // Call this from a button or another script to replay the intro
    public void Replay()
    {
        StopAllCoroutines();
        OnEnable();
    }

    private float Delta() => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    private IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds) { t += Delta(); yield return null; }
    }

    private Vector3 GetPos() => rect != null ? (Vector3)rect.anchoredPosition : transform.localPosition;

    private void SetPos(Vector3 p)
    {
        if (rect != null) rect.anchoredPosition = p;
        else transform.localPosition = p;
    }
}