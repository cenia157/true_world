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

    public void EnableAttackHitbox()
    {
        playerCombat?.EnableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        playerCombat?.DisableAttackHitbox();
    }
}