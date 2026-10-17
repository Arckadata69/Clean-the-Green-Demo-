using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class TrashPickup : MonoBehaviour, IPointerClickHandler
{
    TrashEntry entry;
    bool busy;

    public void Setup(TrashEntry e, int sortingOrder)
    {
        entry = e;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = e.sprite;
        sr.sortingOrder = sortingOrder;

        var box = GetComponent<BoxCollider2D>();
        box.size = e.sprite.bounds.size;
        box.offset = e.sprite.bounds.center;
        name = "Trash - " + e.name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (busy || entry == null) return;

        string id = entry.name.Replace(" ", "_").ToLower();

        if (entry.category == TrashCategory.Collectible)
        {
            PlayerPrefs.SetInt("trophy_" + id, 1);   // permanent almanac trophy
            PlayerData.Collectibles += 1;            // see the note about the shop below
        }
        else if (!SackDisplay.TryAdd(entry))
        {
            StartCoroutine(Shake());                 // sack is full
            return;
        }

        PlayerPrefs.SetInt("picked_" + id, PlayerPrefs.GetInt("picked_" + id, 0) + 1);
        Destroy(gameObject);
    }

    IEnumerator Shake()
    {
        busy = true;
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