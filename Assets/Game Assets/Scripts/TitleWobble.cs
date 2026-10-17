using UnityEngine;

public class TitleWobble : MonoBehaviour
{
    [Header("Sway (rotation)")]
    [SerializeField] float rotateAmount = 3f;
    [SerializeField] float rotateSpeed = 1.5f;

    [Header("Pulse (scale)")]
    [SerializeField] float scaleAmount = 0.03f;
    [SerializeField] float scaleSpeed = 2f;

    [Header("Float (up/down)")]
    [SerializeField] float floatAmount = 8f;
    [SerializeField] float floatSpeed = 1f;

    Vector3 startPos;
    Vector3 startScale;

    void Awake()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        float t = Time.unscaledTime;
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * rotateSpeed) * rotateAmount);
        transform.localScale = startScale * (1f + Mathf.Sin(t * scaleSpeed) * scaleAmount);
        transform.localPosition = startPos + Vector3.up * Mathf.Sin(t * floatSpeed) * floatAmount;
    }
}