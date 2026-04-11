using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("WeaponHitBox")]
    [SerializeField] private WeaponHitbox currentWeaponHitbox;

    public void SetWeaponHitbox(WeaponHitbox weaponHitbox)
    {
        currentWeaponHitbox = weaponHitbox;
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