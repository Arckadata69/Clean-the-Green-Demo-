using System.Collections;
using UnityEngine;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] Transform window;
    [SerializeField] GameObject blocker;
    [SerializeField] float shownY = 0f;
    [SerializeField] float duration = 0.35f;

    public static bool IsOpen;

    float hiddenY;
    Coroutine routine;

    void Start()
    {
        var cam = Camera.main;
        float halfHeight = window.GetComponent<SpriteRenderer>().bounds.extents.y;
        hiddenY = cam.transform.position.y - cam.orthographicSize - halfHeight - 0.5f;

        SetY(hiddenY);
        window.gameObject.SetActive(false);
        blocker.SetActive(false);
        IsOpen = false;
    }

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        window.gameObject.SetActive(true);
        blocker.SetActive(true);
        Play(hiddenY, shownY, false);
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        Play(shownY, hiddenY, true);
    }

    void Play(float from, float to, bool hideAtEnd)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Slide(from, to, hideAtEnd));
    }

    IEnumerator Slide(float from, float to, bool hideAtEnd)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            SetY(Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t)));
            yield return null;
        }
        SetY(to);
        if (hideAtEnd)
        {
            window.gameObject.SetActive(false);
            blocker.SetActive(false);
        }
    }

    void SetY(float y)
    {
        var p = window.position;
        window.position = new Vector3(p.x, y, p.z);
    }
}