using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitBox : WeaponHitBox
{
    private PlayerStat ownerStat;
    private HashSet<MonsterStat> hitMonsters = new HashSet<MonsterStat>();

    public void Init(PlayerStat stat)
    {
        ownerStat = stat;
    }

    public override void EnableHitbox()
    {
        hitMonsters.Clear();
        base.EnableHitbox();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCol == null || !hitCol.enabled) return;
        if (ownerStat == null) return;

        MonsterStat monster = other.GetComponentInParent<MonsterStat>();
        if (monster == null) return;
        if (hitMonsters.Contains(monster)) return;

        hitMonsters.Add(monster);

        int damage = ownerStat.GetStrength();
        monster.TakePhysicalDamage(damage);

        MonsterAI monsterAI = monster.GetComponent<MonsterAI>();
        if (monsterAI != null)
        {
            monsterAI.StartHitReaction(ownerStat.transform);
        }

        SpawnHitEffect(other, transform.position);
    }
}