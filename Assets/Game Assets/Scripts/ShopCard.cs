using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCard : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText, descText, buttonText;
    [SerializeField] Button button;

    ShopItem item;

    public void Setup(ShopItem item, ShopManager shop)
    {
        this.item = item;
        icon.sprite = item.icon;
        nameText.text = item.displayName;
        descText.text = item.type == ItemType.Gloves
            ? $"+{item.scoreMultiplier}x score"
            : $"Capacity: {item.capacity}";
        button.onClick.AddListener(() => shop.OnPressed(item));
        Refresh();
    }

    public void Refresh()
    {
        if (PlayerData.IsEquipped(item))
        {
            buttonText.text = "Equipped";
            button.interactable = false;
        }
        else if (PlayerData.Owns(item))
        {
            buttonText.text = "Equip";
            button.interactable = true;
        }
        else
        {
            buttonText.text = $"Buy\n{item.capCost} Caps &\n{item.collectibleCost} Collectibles";
            button.interactable = PlayerData.CanAfford(item); // greyed out if too poor
        }
    }
}