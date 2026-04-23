using System.Collections.Generic;
using UnityEngine;

public class CorpseLoot : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private List<LootResult> currentLoot = new List<LootResult>();

    private bool isLootGenerated = false;

    public IReadOnlyList<LootResult> CurrentLoot => currentLoot;

    public void GenerateLoot()
    {
        if (isLootGenerated) return;
        if (lootTable == null) return;

        currentLoot = lootTable.RollLoot();
        isLootGenerated = true;
    }

    public bool LootAll(InventoryManager inventory)
    {
        if (inventory == null) return false;
        if (currentLoot == null || currentLoot.Count == 0) return false;

        for (int i = 0; i < currentLoot.Count; i++)
        {
            var loot = currentLoot[i];
            if (loot.itemData == null || loot.amount <= 0) continue;

            bool success = inventory.AddItem(loot.itemData, loot.amount);
            if (!success)
            {
                Debug.Log("인벤토리가 가득 차서 전부 획득하지 못했습니다.");
                return false;
            }
        }

        currentLoot.Clear();
        return true;
    }

    public bool HasLoot()
    {
        return currentLoot != null && currentLoot.Count > 0;
    }
}