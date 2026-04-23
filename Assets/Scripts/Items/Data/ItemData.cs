using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    Material,
    Quest
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Items/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public int ItemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Type")]
    public ItemType itemType;
    public ItemRarity itemRarity;

    [Header("Stack")]
    public bool isStackable = true;
    public int maxStack = 99;

    [Header("Sell / Value")]
    public int price = 0;
}
