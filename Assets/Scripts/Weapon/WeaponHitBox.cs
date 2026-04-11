using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    private Collider hitCol;

    private PlayerStat ownerStat;

    private HashSet<MonsterStat> hitMonsters = new HashSet<MonsterStat>();

    void Start()
    {
        hitCol = GetComponent<Collider>();
        hitCol.enabled = false;
    }

    public void Init(PlayerStat stat)
    {
        ownerStat = stat;
    }

    public void EnableHitbox()
    {
        hitMonsters.Clear();
        hitCol.enabled = true;
    }

    public void DisableHitbox()
    {
        hitCol.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitCol.enabled) return;
        if (ownerStat == null) return;

        MonsterStat monster = other.GetComponent<MonsterStat>();
        if (monster == null) return;

        if (hitMonsters.Contains(monster)) return;

        hitMonsters.Add(monster);

        int damage = ownerStat.GetStrength();
        monster.TakeDamage(damage);
    }
}