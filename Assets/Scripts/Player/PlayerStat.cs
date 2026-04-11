using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    [Header("Stats")]
    [SerializeField] private int strength = 10;
    [SerializeField] private int defense = 5;

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

        Debug.Log($"플레이어 피격! 받은 데미지: {finalDamage}, 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
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

    // 🔥 추가
    public int GetStrength()
    {
        return strength;
    }
}