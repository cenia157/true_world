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

        // 🔥 구르기 무적 등 무적 상태면 데미지도 이펙트도 없음
        if (playerStat.IsInvincible()) return;

        hasHit = true;

        int damage = ownerStat.GetStrength();
        playerStat.TakePhysicalDamage(damage, ownerStat.transform);

        SpawnHitEffect(other, ownerStat.transform.position);
    }
}