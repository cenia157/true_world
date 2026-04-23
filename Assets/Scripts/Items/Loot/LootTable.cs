using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Game/Items/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> entries = new List<LootEntry>();

    public List<LootResult> RollLoot()
    {
        List<LootResult> results = new List<LootResult>();

        foreach (var entry in entries)
        {
            if (entry.itemData == null) continue;
            if (Random.value > entry.dropChance) continue;

            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            if (amount <= 0) continue;

            results.Add(new LootResult(entry.itemData, amount));
        }

        return results;
    }
}