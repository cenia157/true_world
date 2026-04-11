using UnityEngine;

public class MonsterAnimationEvent : MonoBehaviour
{
    private MonsterCombat monsterCombat;
    private MonsterAI monsterAI;

    private void Start()
    {
        monsterCombat = GetComponentInParent<MonsterCombat>();
        monsterAI = GetComponentInParent<MonsterAI>();

        if (monsterCombat == null)
        {
            Debug.LogError("MonsterAnimationEvent: MonsterCombat를 찾지 못했습니다.");
        }

        if (monsterAI == null)
        {
            Debug.LogError("MonsterAnimationEvent: MonsterAI를 찾지 못했습니다.");
        }
    }

    public void EnableAttackHitbox()
    {
        monsterCombat?.EnableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        monsterCombat?.DisableAttackHitbox();
    }

    public void EndAttack()
    {
        monsterAI?.EndAttack();
    }
}