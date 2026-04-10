using UnityEngine;

public class Monster : MonoBehaviour
{
    public int hp = 30;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 피격! 남은 체력: {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject);
    }
}