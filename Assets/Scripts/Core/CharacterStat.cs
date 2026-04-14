using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;

    [SerializeField] private int strength = 10;
    [SerializeField] private int magic = 10;
    [SerializeField] private int defense = 5;

    [SerializeField] private bool isDead = false;

    protected virtual void Start()
    {
        currentHp = maxHp;
        isDead = false;
    }

    public virtual void TakePhysicalDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = CalculateFinalDamage(damage);
        ApplyFinalDamage(finalDamage, "physical");
    }

    public virtual void TakePhysicalDamage(int damage, Transform attacker)
    {
        TakePhysicalDamage(damage);
    }

    public virtual void TakeMagicDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = CalculateFinalDamage(damage);
        ApplyFinalDamage(finalDamage, "magic");
    }

    public int CalculateFinalDamage(int rawDamage)
    {
        return Mathf.Max(rawDamage - defense, 1);
    }

    public void ApplyFinalDamage(int finalDamage, string damageType = "damage")
    {
        if (isDead) return;

        currentHp -= finalDamage;

        Debug.Log($"{gameObject.name} took {finalDamage} {damageType} damage. Current HP: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} died.");
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

    public int GetMagic()
    {
        return magic;
    }

    public int GetDefense()
    {
        return defense;
    }

    public void SetCurrentHp(int value)
    {
        currentHp = Mathf.Clamp(value, 0, maxHp);

        if (currentHp <= 0 && !isDead)
        {
            Die();
        }
    }

    public void SetMaxHp(int value)
    {
        maxHp = Mathf.Max(1, value);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    public void SetStrength(int value)
    {
        strength = Mathf.Max(0, value);
    }

    public void SetMagic(int value)
    {
        magic = Mathf.Max(0, value);
    }

    public void SetDefense(int value)
    {
        defense = Mathf.Max(0, value);
    }

    public void SetDead(bool value)
    {
        isDead = value;
    }
}