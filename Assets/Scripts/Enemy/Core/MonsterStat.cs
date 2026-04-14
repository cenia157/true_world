using UnityEngine;

[RequireComponent(typeof(MonsterDamageHandler))]
public class MonsterStat : CharacterStat
{
    private MonsterDamageHandler damageHandler;

    protected override void Start()
    {
        base.Start();

        damageHandler = GetComponent<MonsterDamageHandler>();

        if (damageHandler == null)
        {
            Debug.LogError("MonsterStat: MonsterDamageHandler를 찾지 못했습니다.");
        }
    }

    public override void TakePhysicalDamage(int damage, Transform attacker)
    {
        if (damageHandler == null)
        {
            Debug.LogWarning("MonsterDamageHandler가 없어 기본 데미지 처리를 수행합니다.");
            base.TakePhysicalDamage(damage, attacker);
            return;
        }

        damageHandler.ReceivePhysicalDamage(damage, attacker);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject);
    }
}