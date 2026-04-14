using UnityEngine;

[RequireComponent(typeof(PlayerStat))]
public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Hit Reaction")]
    [SerializeField] private float defaultHitStunDuration = 0.5f;

    private PlayerStat playerStat;
    private PlayerMovement playerMovement;
    private Animator animator;

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();

        if (playerStat == null)
        {
            Debug.LogError("PlayerDamageHandler: PlayerStat을 찾지 못했습니다.");
        }

        if (playerMovement == null)
        {
            Debug.LogError("PlayerDamageHandler: PlayerMovement를 찾지 못했습니다.");
        }

        if (animator == null)
        {
            Debug.LogError("PlayerDamageHandler: Animator를 찾지 못했습니다.");
        }
    }

    public void ReceivePhysicalDamage(int damage, Transform attacker)
    {
        if (playerStat == null) return;
        if (playerStat.IsDead()) return;
        if (playerStat.IsInvincible()) return;

        int modifiedDamage = damage;
        bool guardSuccess = false;

        if (playerStat.IsGuarding())
        {
            if (attacker != null && IsFrontGuardSuccess(attacker))
            {
                modifiedDamage = Mathf.RoundToInt(damage * playerStat.GetGuardDamageMultiplier());
                guardSuccess = true;
                Debug.Log("정면 가드 성공");
            }
            else
            {
                Debug.Log("가드 중이지만 정면이 아니어서 실패");
            }
        }

        int finalDamage = playerStat.CalculateFinalDamage(modifiedDamage);
        playerStat.ApplyFinalDamage(finalDamage, "physical");

        bool alreadyHitStunned = playerMovement != null && playerMovement.IsHitStunned();

        if (guardSuccess)
        {
            if (!alreadyHitStunned)
            {
                PlayGuardHitAnimation();
            }
        }
        else
        {
            if (!alreadyHitStunned)
            {
                PlayHitAnimation();
                playerMovement?.StartHitStun(defaultHitStunDuration);
            }
        }

        Debug.Log($"플레이어 피격! 받은 데미지: {finalDamage}, 남은 체력: {playerStat.GetCurrentHp()}");
    }

    private bool IsFrontGuardSuccess(Transform attacker)
    {
        Vector3 dirToAttacker = attacker.position - transform.position;
        dirToAttacker.y = 0f;
        dirToAttacker.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        float angle = Vector3.Angle(forward, dirToAttacker);
        return angle <= playerStat.GetGuardAngle() * 0.5f;
    }

    private void PlayHitAnimation()
    {
        if (animator == null) return;

        animator.ResetTrigger("GuardHit");
        animator.ResetTrigger("Hit");
        animator.SetTrigger("Hit");
    }

    private void PlayGuardHitAnimation()
    {
        if (animator == null) return;

        animator.ResetTrigger("Hit");
        animator.ResetTrigger("GuardHit");
        animator.SetTrigger("GuardHit");
    }
}