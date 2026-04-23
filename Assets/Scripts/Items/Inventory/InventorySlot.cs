using System;

[Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public InventorySlot(ItemData itemData, int quantity)
    {
        this.itemData = itemData;
        this.quantity = quantity;
    }

    public bool IsEmpty()
    {
        return itemData == null || quantity <= 0;
    }

    public bool CanStack(ItemData target)
    {
        if (itemData == null || target == null) return false;
        if (itemData != target) return false;
        if (!itemData.isStackable) return false;
        return quantity < itemData.maxStack;
    }

    public int AddAmount(int amount)
    {
        if (itemData == null) return amount;
        if (!itemData.isStackable) return amount;

        int space = itemData.maxStack - quantity;
        int add = Math.Min(space, amount);
        quantity += add;
        return amount - add;
    }
}