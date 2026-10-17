using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SackDisplay : MonoBehaviour
{
    [SerializeField] ShopItem[] bags;          // Plastic Bag, Trash Bag, Sack
    [SerializeField] TMP_Text countText;
    [SerializeField] SpriteRenderer bagIcon;
    [SerializeField] Sprite[] bagSprites;
    [SerializeField] Color normalColor = new Color32(70, 45, 30, 255);
    [SerializeField] Color fullColor = new Color32(220, 50, 50, 255);

    // What is in the sack. Static, so it survives scene changes.
    public static readonly List<TrashEntry> Items = new List<TrashEntry>();

    public static int Carried => Items.Count;
    public static int Capacity { get; private set; } = 10;
    public static bool IsFull => Carried >= Capacity;

    int lastCarried = -1, lastCapacity = -1;

    void Update()
    {
        Capacity = GetCapacity();

        if (Carried == lastCarried && Capacity == lastCapacity) return;
        lastCarried = Carried;
        lastCapacity = Capacity;

        countText.text = Carried + "/" + Capacity;
        countText.color = IsFull ? fullColor : normalColor;
    }

    int GetCapacity()
    {
        int equipped = FindEquippedIndex();
        if (equipped < 0) return 10;

        if (bagIcon != null && bagSprites != null && equipped < bagSprites.Length && bagSprites[equipped] != null)
            bagIcon.sprite = bagSprites[equipped];

        return bags[equipped].capacity;
    }

    int FindEquippedIndex()
    {
        if (bags == null || bags.Length == 0) return -1;

        for (int i = 0; i < bags.Length; i++)
            if (PlayerData.IsEquipped(bags[i])) return i;

        for (int i = 0; i < bags.Length; i++)
            if (bags[i].capCost == 0 && bags[i].collectibleCost == 0) return i;

        return 0;
    }

    public static bool TryAdd(TrashEntry entry)
    {
        if (entry == null || Carried >= Capacity) return false;   // full
        Items.Add(entry);
        return true;
    }

    public static void Remove(TrashEntry entry) { Items.Remove(entry); }
    public static void Empty() { Items.Clear(); }
}