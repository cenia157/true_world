using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxSlotCount = 30;
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        if (slots.Count > 0) return;

        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }

    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0)
            return false;

        int remain = amount;

        if (itemData.isStackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].CanStack(itemData))
                {
                    remain = slots[i].AddAmount(remain);
                    if (remain <= 0)
                        return true;
                }
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                int addAmount = itemData.isStackable
                    ? Mathf.Min(remain, itemData.maxStack)
                    : 1;

                slots[i] = new InventorySlot(itemData, addAmount);
                remain -= addAmount;

                if (remain <= 0)
                    return true;

                if (!itemData.isStackable)
                {
                    while (remain > 0)
                    {
                        int emptyIndex = FindEmptySlot();
                        if (emptyIndex < 0)
                            return false;

                        slots[emptyIndex] = new InventorySlot(itemData, 1);
                        remain--;
                    }

                    return true;
                }
            }
        }

        return false;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
                return i;
        }

        return -1;
    }
}