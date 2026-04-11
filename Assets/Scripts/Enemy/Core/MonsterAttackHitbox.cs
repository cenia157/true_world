using UnityEngine;

public class MonsterAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private Collider hitCol;
    private bool hasHit = false;

    private void Start()
    {
        hitCol = GetComponent<Collider>();

        if (hitCol == null)
        {
            Debug.LogError("MonsterAttackHitbox: Collider를 찾지 못했습니다.");
            return;
        }

        hitCol.enabled = false;
    }

    public void EnableHitbox()
    {
        hasHit = false;

        if (hitCol != null)
        {
            hitCol.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        if (hitCol != null)
        {
            hitCol.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCol == null || !hitCol.enabled) return;
        if (hasHit) return;

        PlayerStat playerStat = other.GetComponentInParent<PlayerStat>();
        if (playerStat == null) return;

        hasHit = true;
        playerStat.TakeDamage(damage);
    }
}