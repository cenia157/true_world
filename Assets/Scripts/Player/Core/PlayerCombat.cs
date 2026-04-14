using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("WeaponHitBox")]
    [SerializeField] private PlayerWeaponHitBox currentWeaponHitbox;

    private PlayerStat playerStat;

    // =========================================================
    // Attack State
    // =========================================================
    private bool isAttackActive = false;

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();

        if (currentWeaponHitbox == null)
        {
            currentWeaponHitbox = GetComponentInChildren<PlayerWeaponHitBox>();
        }

        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.Init(playerStat);
            currentWeaponHitbox.DisableHitbox();
        }
        else
        {
            Debug.LogError("PlayerCombat: PlayerWeaponHitBox를 찾지 못했습니다.");
        }
    }

    // =========================================================
    // Attack Lifecycle
    // =========================================================
    public void StartAttack()
    {
        isAttackActive = true;
        DisableAttackHitbox();

        Debug.Log("PlayerCombat: 공격 시작");
    }

    public void EndAttack()
    {
        if (!isAttackActive)
            return;

        isAttackActive = false;
        DisableAttackHitbox();

        Debug.Log("PlayerCombat: 공격 정상 종료");
    }

    public void CancelAttack()
    {
        isAttackActive = false;
        DisableAttackHitbox();

        Debug.Log("PlayerCombat: 공격 강제 취소");
    }

    // =========================================================
    // Hitbox Control
    // =========================================================
    public void EnableAttackHitbox()
    {
        if (!isAttackActive)
            return;

        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.EnableHitbox();
        }
    }

    public void DisableAttackHitbox()
    {
        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.DisableHitbox();
        }
    }

    public bool IsAttackActive()
    {
        return isAttackActive;
    }
}