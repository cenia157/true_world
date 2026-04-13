using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("WeaponHitBox")]
    [SerializeField] private PlayerWeaponHitBox currentWeaponHitbox;

    private PlayerStat playerStat;

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();

        // 🔥 여기 추가
        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.Init(playerStat);
        }
    }

    public void EnableAttackHitbox()
    {
        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.EnableHitbox();
        }
    }

    public void DisableAttackHitbox()
    {
        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.DisableHitbox();
        }
    }
}