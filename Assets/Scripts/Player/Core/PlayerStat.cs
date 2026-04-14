using UnityEngine;

[RequireComponent(typeof(PlayerDamageHandler))]
public class PlayerStat : CharacterStat
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float currentStamina;

    [Header("Guard")]
    [SerializeField] private float guardAngle = 120f;
    [SerializeField] private float guardDamageMultiplier = 0.7f;

    [Header("State Flags")]
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private bool isGuarding = false;

    private PlayerDamageHandler damageHandler;

    protected override void Start()
    {
        base.Start();

        currentStamina = maxStamina;
        damageHandler = GetComponent<PlayerDamageHandler>();

        if (damageHandler == null)
        {
            Debug.LogError("PlayerStat: PlayerDamageHandler를 찾지 못했습니다.");
        }
    }

    private void Update()
    {
        RegenStamina();
    }

    private void RegenStamina()
    {
        if (IsDead()) return;
        if (currentStamina >= maxStamina) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    public override void TakePhysicalDamage(int damage, Transform attacker)
    {
        if (damageHandler == null)
        {
            Debug.LogWarning("PlayerDamageHandler가 없어 기본 데미지 처리를 수행합니다.");
            base.TakePhysicalDamage(damage, attacker);
            return;
        }

        damageHandler.ReceivePhysicalDamage(damage, attacker);
    }

    public bool TakeStamina(float amount)
    {
        if (IsDead()) return false;
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        return true;
    }

    public void RecoverStamina(float amount)
    {
        if (IsDead()) return;

        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
    }

    public void SetCurrentStamina(float value)
    {
        currentStamina = Mathf.Clamp(value, 0f, maxStamina);
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }

    public float GetStaminaRegenRate()
    {
        return staminaRegenRate;
    }

    public float GetGuardAngle()
    {
        return guardAngle;
    }

    public float GetGuardDamageMultiplier()
    {
        return guardDamageMultiplier;
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
}