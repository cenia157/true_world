using System;

[Serializable]
public class LootResult
{
    public ItemData itemData;
    public int amount;

    public LootResult(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        this.amount = amount;
    }
}