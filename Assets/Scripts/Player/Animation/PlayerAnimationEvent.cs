using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
        playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerCombat == null)
        {
            Debug.LogError("PlayerAnimationEvent: PlayerCombat를 찾지 못했습니다.");
        }

        if (playerMovement == null)
        {
            Debug.LogError("PlayerAnimationEvent: PlayerMovement를 찾지 못했습니다.");
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

    public void EndAttack()
    {
        playerCombat?.EndAttack();
        playerMovement?.NotifyAttackAnimationEnded();
    }
}