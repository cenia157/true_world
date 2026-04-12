using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    [Header("Stats")]
    [SerializeField] private int strength = 10;
    [SerializeField] private int defense = 5;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    private float currentStamina;

    [Header("Guard")]
    [SerializeField] private float guardAngle = 120f;
    [SerializeField] private float guardDamageMultiplier = 0.7f;

    private bool isDead = false;
    private bool isInvincible = false;
    private bool isGuarding = false;

    private void Start()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        int finalDamage = Mathf.Max(damage - defense, 1);
        currentHp -= finalDamage;

        Debug.Log($"플레이어 피격! 받은 데미지: {finalDamage}, 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead) return;
        if (isInvincible) return;

        int finalDamage = damage;

        if (isGuarding && attacker != null && IsFrontGuardSuccess(attacker))
        {
            finalDamage = Mathf.RoundToInt(damage * guardDamageMultiplier);
            Debug.Log("정면 가드 성공");
        }
        else if (isGuarding)
        {
            Debug.Log("가드 중이지만 정면이 아니어서 실패");
        }

        finalDamage = Mathf.Max(finalDamage - defense, 1);
        currentHp -= finalDamage;

        Debug.Log($"플레이어 피격! 받은 데미지: {finalDamage}, 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
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

    public void SetGuarding(bool value)
    {
        isGuarding = value;
    }

    public bool IsGuarding()
    {
        return isGuarding;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("플레이어 사망");
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetCurrentHp()
    {
        return currentHp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public int GetStrength()
    {
        return strength;
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