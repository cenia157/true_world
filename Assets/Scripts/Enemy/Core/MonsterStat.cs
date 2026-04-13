using UnityEngine;

public class MonsterStat : CharacterStat
{
    private MonsterAI monsterAI;

    protected override void Start()
    {
        base.Start();
        monsterAI = GetComponent<MonsterAI>();
    }

    public override void TakePhysicalDamage(int damage, Transform attacker)
    {
        if (isDead) return;

        base.TakePhysicalDamage(damage);
        monsterAI?.StartHitReaction(attacker);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject);
    }
}