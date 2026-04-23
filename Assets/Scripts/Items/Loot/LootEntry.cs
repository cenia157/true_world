using System;
using UnityEngine;

[Serializable]
public class LootEntry
{
    public ItemData itemData;
    public int minAmount = 1;
    public int maxAmount = 1;
    [Range(0f, 1f)] public float dropChance = 1f;
}