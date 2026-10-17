using UnityEngine;

public enum ItemType { Gloves, Bag }

[CreateAssetMenu(menuName = "Shop/Item")]
public class ShopItem : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public ItemType type;
    public int capCost;
    public int collectibleCost;
    public float scoreMultiplier;
    public int capacity;
}