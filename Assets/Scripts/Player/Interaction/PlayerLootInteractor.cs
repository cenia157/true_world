using UnityEngine;

public class PlayerLootInteractor : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float searchRadius = 3f;
    [SerializeField] private LayerMask corpseLayerMask = ~0;
    [SerializeField] private LootUI lootUI;

    private void Start()
    {
        if (lootUI == null)
        {
            lootUI = FindFirstObjectByType<LootUI>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            OpenNearestCorpseLootUI();
        }
    }

    private void OpenNearestCorpseLootUI()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, corpseLayerMask);

        CorpseLoot nearestCorpse = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            CorpseLoot corpseLoot = hit.GetComponentInParent<CorpseLoot>();
            if (corpseLoot == null) continue;
            if (!corpseLoot.CanInteract(transform)) continue;

            float distance = Vector3.Distance(transform.position, corpseLoot.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCorpse = corpseLoot;
            }
        }

        if (nearestCorpse == null)
        {
            Debug.Log("근처에 확인 가능한 시체가 없습니다.");
            lootUI?.Close();
            return;
        }

        lootUI?.Open(nearestCorpse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}