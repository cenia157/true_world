using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class MonsterDamageHandler : MonoBehaviour
{
    private MonsterStat monsterStat;
    private MonsterAI monsterAI;

    private void Awake()
    {
        monsterStat = GetComponent<MonsterStat>();
        monsterAI = GetComponent<MonsterAI>();

        if (monsterStat == null)
        {
            Debug.LogError("MonsterDamageHandler: MonsterStat을 찾지 못했습니다.");
        }

        if (monsterAI == null)
        {
            Debug.LogError("MonsterDamageHandler: MonsterAI를 찾지 못했습니다.");
        }
    }

    public void ReceivePhysicalDamage(int damage, Transform attacker)
    {
        if (monsterStat == null) return;
        if (monsterStat.IsDead()) return;

        int finalDamage = monsterStat.CalculateFinalDamage(damage);
        monsterStat.ApplyFinalDamage(finalDamage, "physical");

        if (!monsterStat.IsDead())
        {
            monsterAI?.StartHitReaction(attacker);
        }

        Debug.Log($"{gameObject.name} 피격! 받은 데미지: {finalDamage}, 남은 체력: {monsterStat.GetCurrentHp()}");
    }
}