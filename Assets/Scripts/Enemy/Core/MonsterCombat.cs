using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [SerializeField] private MonsterAttackHitbox attackHitbox;

    private MonsterStat monsterStat;

    private void Start()
    {
        monsterStat = GetComponent<MonsterStat>();

        if (attackHitbox == null)
        {
            Debug.LogError("MonsterCombat: MonsterAttackHitbox가 연결되지 않았습니다.");
            return;
        }

        if (monsterStat == null)
        {
            Debug.LogError("MonsterCombat: MonsterStat을 찾지 못했습니다.");
            return;
        }

        attackHitbox.Init(monsterStat);
    }

    public void EnableAttackHitbox()
    {
        attackHitbox?.EnableHitbox();
    }

    public void DisableAttackHitbox()
    {
        attackHitbox?.DisableHitbox();
    }
}