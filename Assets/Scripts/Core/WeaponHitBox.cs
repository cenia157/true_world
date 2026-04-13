using UnityEngine;

public abstract class WeaponHitBox : MonoBehaviour
{
    protected Collider hitCol;

    [SerializeField] protected GameObject hitEffectPrefab;

    protected virtual void Start()
    {
        hitCol = GetComponent<Collider>();

        if (hitCol == null)
        {
            Debug.LogError($"{gameObject.name}: Collider를 찾지 못했습니다.");
            return;
        }

        hitCol.enabled = false;
    }

    public virtual void EnableHitbox()
    {
        if (hitCol != null)
            hitCol.enabled = true;
    }

    public virtual void DisableHitbox()
    {
        if (hitCol != null)
            hitCol.enabled = false;
    }

    protected void SpawnHitEffect(Collider other, Vector3 attackerPos)
    {
        if (hitEffectPrefab == null) return;

        Vector3 hitPoint = other.ClosestPoint(attackerPos) + Vector3.up * 0.5f;
        Quaternion rot = Quaternion.LookRotation((other.transform.position - attackerPos).normalized);

        Instantiate(hitEffectPrefab, hitPoint, rot);
    }
}