using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ShopItem[] items;      // order: gloves first, then bags
    [SerializeField] ShopCard cardPrefab;
    [SerializeField] TMP_Text headerPrefab;
    [SerializeField] Transform content;

    readonly List<ShopCard> cards = new();

    void Start()
    {
        // Starter items: equip the first free item of each type if nothing is equipped yet
        foreach (var i in items)
            if (i.capCost == 0 && i.collectibleCost == 0 && !HasEquipped(i.type))
                PlayerData.Equip(i);

        bool first = true;
        ItemType lastType = default;
        foreach (var i in items)
        {
            if (first || i.type != lastType)
            {
                var h = Instantiate(headerPrefab, content);
                h.text = i.type == ItemType.Gloves ? "Equipment" : "Bag";
                lastType = i.type;
                first = false;
            }
            var card = Instantiate(cardPrefab, content);
            card.Setup(i, this);
            cards.Add(card);
        }
    }

    bool HasEquipped(ItemType t) => PlayerPrefs.GetString("equip_" + t, "") != "";

    public void OnPressed(ShopItem item)
    {
        if (!PlayerData.Owns(item))
        {
            if (!PlayerData.CanAfford(item)) return;
            PlayerData.Caps -= item.capCost;
            PlayerData.Collectibles -= item.collectibleCost;
            PlayerData.SetOwned(item);
        }
        PlayerData.Equip(item);          // buying auto-equips; Equip button swaps
        foreach (var c in cards) c.Refresh();
    }
}