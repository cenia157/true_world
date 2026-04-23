using System.Collections.Generic;
using UnityEngine;

public class CorpseLoot : MonoBehaviour
{
    [Header("Loot")]
    [SerializeField] private LootTable lootTable;
    [SerializeField] private List<LootResult> currentLoot = new List<LootResult>();

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2.5f;

    private bool isLootGenerated = false;

    public IReadOnlyList<LootResult> CurrentLoot => currentLoot;

    public void GenerateLoot()
    {
        if (isLootGenerated) return;
        if (lootTable == null) return;

        currentLoot = lootTable.RollLoot();
        isLootGenerated = true;

        Debug.Log($"{gameObject.name}: 루팅 아이템 생성 완료, 개수 = {currentLoot.Count}");
    }

    public bool CanInteract(Transform player)
    {
        if (player == null) return false;
        if (!HasLoot()) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= interactDistance;
    }

    public bool LootAll(InventoryManager inventory)
    {
        if (inventory == null) return false;
        if (currentLoot == null || currentLoot.Count == 0) return false;

        for (int i = 0; i < currentLoot.Count; i++)
        {
            LootResult loot = currentLoot[i];

            if (loot.itemData == null || loot.amount <= 0)
                continue;

            bool success = inventory.AddItem(loot.itemData, loot.amount);

            if (!success)
            {
                Debug.Log("인벤토리가 가득 차서 전부 획득하지 못했습니다.");
                return false;
            }

            Debug.Log($"{loot.itemData.itemName} x{loot.amount} 획득");
        }

        currentLoot.Clear();
        Debug.Log($"{gameObject.name}: 루팅 완료");

        return true;
    }

    public bool HasLoot()
    {
        return currentLoot != null && currentLoot.Count > 0;
    }
}