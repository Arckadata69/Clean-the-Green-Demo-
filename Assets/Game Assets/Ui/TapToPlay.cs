using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TapToPlay : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";   // exact scene name
    [Tooltip("Ignore taps until the title has finished dropping")]
    [SerializeField] private float enableAfterSeconds = 0f;

    private float timer;
    private bool started;

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (started || timer < enableAfterSeconds || !Tapped()) return;

        started = true;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(gameSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    private bool Tapped()
    {
#if ENABLE_INPUT_SYSTEM
        return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
#else
        return Input.GetMouseButtonDown(0)
            || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
#endif
    }
}