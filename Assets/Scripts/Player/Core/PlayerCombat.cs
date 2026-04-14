using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Sword HitBox")]
    [SerializeField] private PlayerWeaponHitBox currentWeaponHitbox;

    [Header("Kick HitBox")]
    [SerializeField] private PlayerWeaponHitBox kickHitbox;

    [Header("Combo")]
    [SerializeField] private int maxComboStep = 3;

    [Header("Stamina Cost")]
    [SerializeField] private float attack1StaminaCost = 15f;
    [SerializeField] private float attack2StaminaCost = 15f;
    [SerializeField] private float attack3StaminaCost = 20f;

    private PlayerStat playerStat;
    private PlayerStateController stateController;
    private Animator animator;

    private bool isAttackActive = false;

    private int currentComboStep = 0;
    private bool nextComboRequested = false;
    private bool comboWindowOpen = false;

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
        animator = GetComponentInChildren<Animator>();
        stateController = GetComponent<PlayerStateController>();

        if (currentWeaponHitbox == null)
        {
            currentWeaponHitbox = GetComponentInChildren<PlayerWeaponHitBox>();
        }

        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.Init(playerStat);
            currentWeaponHitbox.DisableHitbox();
        }

        if (kickHitbox != null)
        {
            kickHitbox.Init(playerStat);
            kickHitbox.DisableHitbox();
        }

        if (currentWeaponHitbox == null)
        {
            Debug.LogError("PlayerCombat: PlayerWeaponHitBox를 찾지 못했습니다.");
        }

        if (stateController == null)
        {
            Debug.LogError("PlayerCombat: PlayerStateController를 찾지 못했습니다.");
        }

        if (animator == null)
        {
            Debug.LogError("PlayerCombat: Animator를 찾지 못했습니다.");
        }
    }

    public void OnAttackButtonPressed()
    {
        if (stateController == null || playerStat == null)
            return;

        if (stateController.CanAttack())
        {
            StartComboAttack(1);
            return;
        }

        if (stateController.IsState(PlayerState.Attack) && comboWindowOpen)
        {
            nextComboRequested = true;
            Debug.Log("PlayerCombat: 다음 콤보 입력 예약");
        }
    }

    private void StartComboAttack(int comboStep)
    {
        if (comboStep < 1 || comboStep > maxComboStep)
            return;

        float staminaCost = GetComboStaminaCost(comboStep);

        if (!playerStat.TakeStamina(staminaCost))
        {
            Debug.Log($"PlayerCombat: {comboStep}타 스태미너 부족");
            ResetCombo();
            return;
        }

        currentComboStep = comboStep;
        nextComboRequested = false;
        comboWindowOpen = false;
        isAttackActive = true;

        DisableAttackHitbox();
        ResetAttackTriggers();

        stateController.ChangeState(PlayerState.Attack);

        if (animator == null)
            return;

        animator.SetBool("isGuarding", false);

        switch (currentComboStep)
        {
            case 1:
                animator.SetTrigger("Attack1");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
            case 3:
                animator.SetTrigger("Attack3");
                break;
        }

        Debug.Log($"PlayerCombat: {currentComboStep}타 시작");
    }

    private float GetComboStaminaCost(int comboStep)
    {
        switch (comboStep)
        {
            case 1: return attack1StaminaCost;
            case 2: return attack2StaminaCost;
            case 3: return attack3StaminaCost;
            default: return attack1StaminaCost;
        }
    }

    public void OpenComboWindow()
    {
        comboWindowOpen = true;
        Debug.Log("PlayerCombat: 콤보 입력 가능");
    }

    public void CloseComboWindow()
    {
        comboWindowOpen = false;
        Debug.Log("PlayerCombat: 콤보 입력 종료");
    }

    public void OnAttackAnimationEnd()
    {
        // 이미 피격 등으로 취소된 공격이면 무시
        if (!isAttackActive)
            return;

        DisableAttackHitbox();
        isAttackActive = false;
        comboWindowOpen = false;

        if (nextComboRequested && currentComboStep < maxComboStep)
        {
            StartComboAttack(currentComboStep + 1);
            return;
        }

        ResetCombo();
    }

    public void EndAttack()
    {
        DisableAttackHitbox();
        isAttackActive = false;
        ResetAttackTriggers();
        ResetCombo();
    }

    public void CancelAttack()
    {
        DisableAttackHitbox();
        isAttackActive = false;
        comboWindowOpen = false;
        nextComboRequested = false;

        ResetAttackTriggers();
        ResetCombo();

        Debug.Log("PlayerCombat: 공격 강제 취소");
    }

    private void ResetAttackTriggers()
    {
        if (animator == null)
            return;

        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
    }

    public void EnableAttackHitbox()
    {
        if (!isAttackActive)
            return;

        DisableAttackHitbox();

        switch (currentComboStep)
        {
            case 1:
                currentWeaponHitbox?.EnableHitbox();
                break;

            case 2:
                kickHitbox?.EnableHitbox();
                break;

            case 3:
                currentWeaponHitbox?.EnableHitbox();
                break;
        }
    }

    public void DisableAttackHitbox()
    {
        currentWeaponHitbox?.DisableHitbox();
        kickHitbox?.DisableHitbox();
    }

    public void ResetCombo()
    {
        currentComboStep = 0;
        nextComboRequested = false;
        comboWindowOpen = false;
        isAttackActive = false;

        if (stateController != null && stateController.IsState(PlayerState.Attack))
        {
            stateController.ChangeState(PlayerState.Idle);
        }

        Debug.Log("PlayerCombat: 콤보 종료");
    }

    public bool IsAttackActive()
    {
        return isAttackActive;
    }

    public int GetCurrentComboStep()
    {
        return currentComboStep;
    }
}