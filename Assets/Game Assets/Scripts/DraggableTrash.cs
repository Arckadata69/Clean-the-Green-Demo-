using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class DraggableTrash : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TrashEntry Entry { get; private set; }
    public System.Action<DraggableTrash, Bin> onDropped;   // bin is null if dropped on nothing

    Vector3 home, grabOffset;
    bool dragging, locked;
    Camera cam;

    public void Setup(TrashEntry entry, Vector3 homePos, float size, int sortingOrder)
    {
        Entry = entry;
        home = homePos;
        cam = Camera.main;

        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = entry.sprite;
        sr.sortingOrder = sortingOrder;

        float longest = Mathf.Max(entry.sprite.bounds.size.x, entry.sprite.bounds.size.y);
        transform.localScale = Vector3.one * (size / longest);
        transform.position = homePos;

        var box = GetComponent<BoxCollider2D>();
        box.size = entry.sprite.bounds.size;
        box.offset = entry.sprite.bounds.center;
        name = "Dragging - " + entry.name;
    }

    Vector3 PointerWorld(PointerEventData e)
    {
        Vector3 p = cam.ScreenToWorldPoint(e.position);
        p.z = 0f;
        return p;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (locked) return;
        dragging = true;
        grabOffset = transform.position - PointerWorld(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (!dragging) return;
        transform.position = PointerWorld(e) + grabOffset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!dragging) return;
        dragging = false;

        Bin bin = null;
        foreach (var hit in Physics2D.OverlapPointAll(transform.position))
        {
            bin = hit.GetComponent<Bin>();
            if (bin != null) break;
        }
        onDropped?.Invoke(this, bin);
    }

    public void ReturnHome() { StartCoroutine(Glide()); }

    IEnumerator Glide()
    {
        locked = true;
        Vector3 from = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / 0.2f;
            transform.position = Vector3.Lerp(from, home, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = home;
        locked = false;
    }

    public void Consume(Vector3 target)
    {
        locked = true;
        StartCoroutine(Fly(target));
    }

    IEnumerator Fly(Vector3 target)
    {
        Vector3 from = transform.position, s = transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / 0.15f;
            transform.position = Vector3.Lerp(from, target, t);
            transform.localScale = Vector3.Lerp(s, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}