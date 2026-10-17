using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Bin : MonoBehaviour
{
    public TrashCategory accepts;
    [SerializeField] float shakeAmount = 0.12f;   // world units

    Vector3 restPos, restScale;
    Coroutine shakeRoutine, popRoutine;

    void Awake()
    {
        restPos = transform.position;
        restScale = transform.localScale;
    }

    public void ShakeWrong()
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        transform.position = restPos;
        shakeRoutine = StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.unscaledDeltaTime;
            float fade = 1f - t / 0.4f;
            transform.position = restPos + new Vector3(Mathf.Sin(t * 70f) * shakeAmount * fade, 0f, 0f);
            yield return null;
        }
        transform.position = restPos;
    }

    public void PopCorrect()
    {
        if (popRoutine != null) StopCoroutine(popRoutine);
        transform.localScale = restScale;
        popRoutine = StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = restScale * (1f + Mathf.Sin(t / 0.2f * Mathf.PI) * 0.08f);
            yield return null;
        }
        transform.localScale = restScale;
    }
}