using UnityEngine;

public class PlayerStat : CharacterStat
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    private float currentStamina;

    [Header("Guard")]
    [SerializeField] private float guardAngle = 120f;
    [SerializeField] private float guardDamageMultiplier = 0.7f;

    private bool isInvincible = false;
    private bool isGuarding = false;

    private Animator animator;
    private PlayerMovement playerMovement;

    protected override void Start()
    {
        base.Start();

        currentStamina = maxStamina;

        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (animator == null)
        {
            Debug.LogError("PlayerStat: Animator를 찾지 못했습니다.");
        }
    }

    private void Update()
    {
        RegenStamina();
    }

    private void RegenStamina()
    {
        if (isDead) return;
        if (currentStamina >= maxStamina) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    public override void TakePhysicalDamage(int damage, Transform attacker)
    {
        if (isDead) return;
        if (isInvincible) return;

        int finalDamage = damage;
        bool guardSuccess = false;

        if (isGuarding && attacker != null && IsFrontGuardSuccess(attacker))
        {
            finalDamage = Mathf.RoundToInt(damage * guardDamageMultiplier);
            guardSuccess = true;
            Debug.Log("정면 가드 성공");
        }
        else if (isGuarding)
        {
            Debug.Log("가드 중이지만 정면이 아니어서 실패");
        }

        finalDamage = Mathf.Max(finalDamage - defense, 1);
        currentHp -= finalDamage;

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
                playerMovement?.StartHitStun(0.5f);
            }
        }

        Debug.Log($"플레이어 피격! 받은 데미지: {finalDamage}, 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
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

    private bool IsFrontGuardSuccess(Transform attacker)
    {
        Vector3 dirToAttacker = attacker.position - transform.position;
        dirToAttacker.y = 0f;
        dirToAttacker.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        float angle = Vector3.Angle(forward, dirToAttacker);

        return angle <= guardAngle * 0.5f;
    }

    public bool TakeStamina(float amount)
    {
        if (isDead) return false;
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        return true;
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    public void SetGuarding(bool value)
    {
        isGuarding = value;
    }

    public bool IsGuarding()
    {
        return isGuarding;
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("플레이어 사망");
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }
}