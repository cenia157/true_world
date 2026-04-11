using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [SerializeField] private MonsterAttackHitbox attackHitbox;

    private void Start()
    {
        if (attackHitbox == null)
        {
            Debug.LogError("MonsterCombat: MonsterAttackHitbox가 연결되지 않았습니다.");
        }
    }

    public void EnableAttackHitbox()
    {
        attackHitbox?.EnableHitbox();
    }

    public void DisableAttackHitbox()
    {
        attackHitbox?.DisableHitbox();
    }
}