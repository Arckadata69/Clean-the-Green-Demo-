using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MiniGameButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] string sceneName = "Mini Game";
    [SerializeField] bool requireFullSack = true;   // untick to allow it with any trash in the sack
    [SerializeField] bool pulseWhenReady = true;    // icon gently pulses when it can be used

    Vector3 baseScale;
    bool busy;

    void Awake() { baseScale = transform.localScale; }

    bool IsReady() => requireFullSack ? SackDisplay.IsFull : SackDisplay.Carried > 0;

    void Update()
    {
        if (busy) return;
        float s = (pulseWhenReady && IsReady())
            ? 1f + Mathf.Sin(Time.unscaledTime * 5f) * 0.06f
            : 1f;
        transform.localScale = baseScale * s;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (busy) return;

        if (IsReady()) SceneManager.LoadScene(sceneName);
        else StartCoroutine(Shake());               // sack isn't full yet
    }

    IEnumerator Shake()
    {
        busy = true;
        transform.localScale = baseScale;
        Vector3 start = transform.localPosition;
        float t = 0f;
        while (t < 0.25f)
        {
            t += Time.unscaledDeltaTime;
            transform.localPosition = start + new Vector3(Mathf.Sin(t * 60f) * 0.05f, 0f, 0f);
            yield return null;
        }
        transform.localPosition = start;
        busy = false;
    }
}