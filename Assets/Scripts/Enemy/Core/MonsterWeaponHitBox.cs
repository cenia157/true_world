using UnityEngine;

public class MonsterWeaponHitBox : WeaponHitBox
{
    private bool hasHit = false;
    private MonsterStat ownerStat;

    public void Init(MonsterStat stat)
    {
        ownerStat = stat;
    }

    public override void EnableHitbox()
    {
        hasHit = false;
        base.EnableHitbox();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCol == null || !hitCol.enabled) return;
        if (hasHit) return;
        if (ownerStat == null) return;
        if (ownerStat.IsDead()) return;

        PlayerStat playerStat = other.GetComponentInParent<PlayerStat>();
        if (playerStat == null) return;
        if (playerStat.IsDead()) return;

        hasHit = true;

        int damage = ownerStat.GetStrength();
        playerStat.TakePhysicalDamage(damage, ownerStat.transform);

        SpawnHitEffect(other, ownerStat.transform.position);
    }
}