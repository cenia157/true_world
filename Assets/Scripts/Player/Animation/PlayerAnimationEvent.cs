using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    [SerializeField] private WeaponHitbox weaponHitbox;

    void Start()
    {
        if (weaponHitbox == null)
        {
            Debug.LogError("PlayerAnimationEvent: WeaponHitbox가 연결되지 않았습니다.");
        }
    }

    public void EnableAttackHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.EnableHitbox();
    }

    public void DisableAttackHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.DisableHitbox();
    }
}