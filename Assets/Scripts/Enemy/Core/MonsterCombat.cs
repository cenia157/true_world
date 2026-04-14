using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [SerializeField] private MonsterWeaponHitBox attackHitbox;

    private MonsterStat monsterStat;

    // =========================================================
    // Attack State
    // =========================================================
    private bool isAttackActive = false;

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
        attackHitbox.DisableHitbox();
    }

    // =========================================================
    // Attack Lifecycle
    // =========================================================
    public void StartAttack()
    {
        isAttackActive = true;
        DisableAttackHitbox();

        Debug.Log("MonsterCombat: 공격 시작");
    }

    public void EndAttack()
    {
        if (!isAttackActive)
            return;

        isAttackActive = false;
        DisableAttackHitbox();

        Debug.Log("MonsterCombat: 공격 정상 종료");
    }

    public void CancelAttack()
    {
        isAttackActive = false;
        DisableAttackHitbox();

        Debug.Log("MonsterCombat: 공격 강제 취소");
    }

    // =========================================================
    // Hitbox Control
    // =========================================================
    public void EnableAttackHitbox()
    {
        if (!isAttackActive)
            return;

        attackHitbox?.EnableHitbox();
    }

    public void DisableAttackHitbox()
    {
        attackHitbox?.DisableHitbox();
    }

    public bool IsAttackActive()
    {
        return isAttackActive;
    }
}