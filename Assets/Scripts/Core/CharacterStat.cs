using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHp = 100;
    public int currentHp;

    public int strength = 10;
    public int magic = 10;
    public int defense = 5;

    public bool isDead = false;

    protected virtual void Start()
    {
        currentHp = maxHp;
    }

    public virtual void TakePhysicalDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = Mathf.Max(damage - defense, 1);
        currentHp -= finalDamage;

        Debug.Log($"{gameObject.name} took {finalDamage} physical damage. Current HP: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    public virtual void TakePhysicalDamage(int damage, Transform attacker)
    {
        TakePhysicalDamage(damage);
    }

    public virtual void TakeMagicDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = Mathf.Max(damage - defense, 1);
        currentHp -= finalDamage;

        Debug.Log($"{gameObject.name} took {finalDamage} magic damage. Current HP: {currentHp}");

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
}