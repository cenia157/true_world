using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;

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
        Debug.Log($"플레이어 피격! 받은 데미지: {damage}, 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
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
}