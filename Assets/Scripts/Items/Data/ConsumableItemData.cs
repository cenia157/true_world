using UnityEngine;

public enum ConsumableEffectType
{
    HealHp,
    HealStamina
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Game/Items/Consumable Item")]
public class ConsumableItemData : ItemData
{
    [Header("Consumable")]
    public ConsumableEffectType effectType;
    public int effectValue = 10;

    private void OnValidate()
    {
        itemType = ItemType.Consumable;
    }
}