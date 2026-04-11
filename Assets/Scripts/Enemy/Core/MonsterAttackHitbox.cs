using UnityEngine;

public class MonsterAttackHitbox : MonoBehaviour
{
    private Collider hitCol;
    private bool hasHit = false;

    private MonsterStat ownerStat;

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

    public void Init(MonsterStat stat)
    {
        ownerStat = stat;
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
        if (ownerStat == null) return;
        if (ownerStat.IsDead()) return;

        PlayerStat playerStat = other.GetComponentInParent<PlayerStat>();
        if (playerStat == null) return;
        if (playerStat.IsDead()) return;

        hasHit = true;

        int damage = ownerStat.GetStrength();
        playerStat.TakeDamage(damage);
    }
}