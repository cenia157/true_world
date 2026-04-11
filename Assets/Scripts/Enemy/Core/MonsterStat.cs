using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 30;
    private int currentHp;

    [Header("Stats")]
    [SerializeField] private int strength = 10;
    [SerializeField] private int defense = 2;

    private bool isDead = false;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = Mathf.Max(damage - defense, 1);
        currentHp -= finalDamage;

        Debug.Log($"{gameObject.name} 피격! 받은 데미지: {finalDamage}, 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} 사망");

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetCurrentHp()
    {
        return currentHp;
    }

    public int GetStrength()
    {
        return strength;
    }
}