using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Helmet,
    Armor,
    Gloves,
    Boots,
    Ring,
    Necklace
}

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Game/Items/Equipment Item")]
public class EquipmentItemData : ItemData
{
    [Header("Equipment")]
    public EquipmentType equipmentType;

    [Header("Bonus Stats")]
    public int bonusHp;
    public int bonusStrength;
    public int bonusMagic;
    public int bonusDefense;

    private void OnValidate()
    {
        itemType = ItemType.Equipment;
        isStackable = false;
        maxStack = 1;
    }
}