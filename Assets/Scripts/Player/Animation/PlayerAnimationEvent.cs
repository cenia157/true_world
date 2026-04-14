using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();

        if (playerCombat == null)
        {
            Debug.LogError("PlayerAnimationEvent: PlayerCombat를 찾지 못했습니다.");
        }
    }

    public void EnableWeaponHitbox()
    {
        playerCombat?.EnableAttackHitbox();
    }

    public void DisableWeaponHitbox()
    {
        playerCombat?.DisableAttackHitbox();
    }

    public void OpenComboWindow()
    {
        playerCombat?.OpenComboWindow();
    }

    public void CloseComboWindow()
    {
        playerCombat?.CloseComboWindow();
    }

    public void EndAttack()
    {
        playerCombat?.OnAttackAnimationEnd();
    }
}