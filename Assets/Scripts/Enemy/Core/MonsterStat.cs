using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    [SerializeField] private int maxHp = 30;

    private int currentHp;
    private bool isDead = false;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
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
}