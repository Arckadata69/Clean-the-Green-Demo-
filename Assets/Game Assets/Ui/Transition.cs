using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Sprite transitionSprite;      // your dash2 image
    [SerializeField] private float coverDuration = 0.8f;   // falling in
    [SerializeField] private float holdTime = 0.1f;        // pause while covered
    [SerializeField] private float revealDuration = 0.8f;  // falling out
    [Tooltip("Slow start then fast = feels like falling")]
    [SerializeField] private AnimationCurve coverCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 2f, 0f));
    [SerializeField] private AnimationCurve revealCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private Canvas canvas;
    private RectTransform panel;
    private bool busy;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // survives the scene change
        Build();
    }

    private void Build()
    {
        var canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform, false);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;      // above everything

        var panelGO = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panelGO.transform.SetParent(canvasGO.transform, false);
        panel = panelGO.GetComponent<RectTransform>();
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.offsetMin = panel.offsetMax = Vector2.zero;

        var img = panelGO.GetComponent<Image>();
        img.sprite = transitionSprite;
        img.preserveAspect = false;       // stretch to fill the screen
        img.raycastTarget = true;         // blocks taps during the transition

        panelGO.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if (!busy) StartCoroutine(Run(sceneName));
    }

    private IEnumerator Run(string sceneName)
    {
        busy = true;
        float h = Mathf.Max(((RectTransform)canvas.transform).rect.height, Screen.height);

        panel.anchoredPosition = new Vector2(0f, h);   // start just above the screen
        panel.gameObject.SetActive(true);

        yield return Move(h, 0f, coverDuration, coverCurve);   // fall and cover
        yield return new WaitForSecondsRealtime(holdTime);

        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        yield return null;                                      // let the new scene draw once

        yield return Move(0f, -h, revealDuration, revealCurve); // keep falling and reveal

        panel.gameObject.SetActive(false);
        busy = false;
    }

    private IEnumerator Move(float from, float to, float duration, AnimationCurve curve)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(duration, 0.01f);
            float y = Mathf.LerpUnclamped(from, to, curve.Evaluate(Mathf.Clamp01(t)));
            panel.anchoredPosition = new Vector2(0f, y);
            yield return null;
        }
    }
}