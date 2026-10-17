using UnityEngine;

public static class PlayerData
{
    public static int Caps
    {
        get => PlayerPrefs.GetInt("caps", 0);
        set => PlayerPrefs.SetInt("caps", value);
    }
    public static int Collectibles
    {
        get => PlayerPrefs.GetInt("collectibles", 0);
        set => PlayerPrefs.SetInt("collectibles", value);
    }

    public static bool Owns(ShopItem i) =>
        PlayerPrefs.GetInt("own_" + i.id, (i.capCost == 0 && i.collectibleCost == 0) ? 1 : 0) == 1;

    public static void SetOwned(ShopItem i) => PlayerPrefs.SetInt("own_" + i.id, 1);

    public static bool IsEquipped(ShopItem i) =>
        PlayerPrefs.GetString("equip_" + i.type, "") == i.id;

    public static void Equip(ShopItem i)
    {
        PlayerPrefs.SetString("equip_" + i.type, i.id);
        PlayerPrefs.Save();
    }

    public static bool CanAfford(ShopItem i) =>
        Caps >= i.capCost && Collectibles >= i.collectibleCost;
}