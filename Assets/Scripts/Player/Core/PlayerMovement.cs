using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStateController))]
public class PlayerMovement : MonoBehaviour
{
    // =========================================================
    // References Layer
    // =========================================================
    private Rigidbody rb;
    private Animator animator;
    private PlayerCameraController cameraController;
    private PlayerStat playerStat;
    private PlayerStateController stateController;
    private PlayerCombat playerCombat;

    // =========================================================
    // Input Cache Layer
    // =========================================================
    private Vector3 moveInput;
    private Vector3 moveDirection;
    private Vector3 dodgeDirection;

    // =========================================================
    // Runtime Cache Layer
    // - 이동 표현용 캐시
    // =========================================================
    private bool isRun;

    private bool runLocked = false;
    private bool guardLocked = false;

    // =========================================================
    // Timer Layer
    // =========================================================
    private float lastDodgeTime = -999f;
    private float dodgeEndTime;
    private float attackEndTime;
    private float guardEndTime;
    private float hitStunEndTime = -999f;

    // =========================================================
    // Movement Config Layer
    // =========================================================
    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 16f;

    // =========================================================
    // Run Config Layer
    // =========================================================
    [Header("Run Stamina")]
    [SerializeField] private float runStaminaCostPerSecond = 25f;
    [SerializeField] private float minStaminaToStartRun = 20f;

    // =========================================================
    // Dodge Config Layer
    // =========================================================
    [Header("Dodge")]
    [SerializeField] private float dodgeCooldown = 0.5f;
    [SerializeField] private float dodgeSpeed = 6f;
    [SerializeField] private float dodgeDuration = 1.25f;
    [SerializeField] private float dodgeStaminaCost = 30f;

    // =========================================================
    // Attack Config Layer
    // =========================================================
    [Header("Attack")]
    [SerializeField] private float attackDuration = 1.0f;
    [SerializeField] private float attackStaminaCost = 15f;

    // =========================================================
    // Guard Config Layer
    // =========================================================
    [Header("Guard")]
    [SerializeField] private float guardStaminaCostPerSecond = 50f;
    [SerializeField] private float guardDuration = 1.0f;
    [SerializeField] private float minStaminaToStartGuard = 25f;

    // =========================================================
    // HitStun Config Layer
    // =========================================================
    [Header("HitStun")]
    [SerializeField] private float hitStunDuration = 0.5f;

    // =========================================================
    // Unity Lifecycle Layer
    // =========================================================
    private void Start()
    {
        InitializeReferences();
        ValidateReferences();
    }

    private void Update()
    {
        HandleStateTimer();
        HandleInput();
        UpdateLocomotionState();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        HandleMovementPhysics();
    }

    // =========================================================
    // Initialize Layer
    // =========================================================
    private void InitializeReferences()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        cameraController = Camera.main != null ? Camera.main.GetComponent<PlayerCameraController>() : null;
        playerStat = GetComponent<PlayerStat>();
        stateController = GetComponent<PlayerStateController>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void ValidateReferences()
    {
        if (animator == null)
        {
            Debug.LogError("Animator를 찾지 못했습니다.");
        }

        if (cameraController == null)
        {
            Debug.LogError("Main Camera에 PlayerCameraController가 없습니다.");
        }

        if (playerStat == null)
        {
            Debug.LogError("PlayerStat을 찾지 못했습니다.");
        }

        if (stateController == null)
        {
            Debug.LogError("PlayerStateController를 찾지 못했습니다.");
        }

        if (playerCombat == null)
        {
            Debug.LogError("PlayerCombat를 찾지 못했습니다.");
        }
    }

    // =========================================================
    // Timer Layer
    // =========================================================
    private void HandleStateTimer()
    {
        if (stateController.IsState(PlayerState.Dodge) && Time.time >= dodgeEndTime)
        {
            EndDodge();
        }

        if (stateController.IsState(PlayerState.Attack) && Time.time >= attackEndTime)
        {
            EndAttack();
        }

        if (stateController.IsState(PlayerState.Guard) && Time.time >= guardEndTime)
        {
            EndGuard();
        }

        if (stateController.IsState(PlayerState.Hit) && Time.time >= hitStunEndTime)
        {
            EndHitStun();
        }
    }

    // =========================================================
    // Input Layer
    // =========================================================
    private void HandleInput()
    {
        if (stateController.IsState(PlayerState.Dead))
            return;

        if (stateController.IsState(PlayerState.Hit))
            return;

        UpdateGuardLock();
        UpdateRunLock();

        ReadMoveInput();
        UpdateMoveDirection();
        UpdateRunState();

        HandleGuardStamina();
        HandleRunStamina();

        HandleGuardInput();

        if (stateController.IsState(PlayerState.Dodge) ||
            stateController.IsState(PlayerState.Attack) ||
            stateController.IsState(PlayerState.Guard))
        {
            return;
        }

        HandleDodgeInput();
        HandleAttackInput();
    }

    private void HandleGuardInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryStartGuard();
        }
    }

    private void ReadMoveInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(x, 0f, z).normalized;
    }

    private void UpdateRunState()
    {
        if (!stateController.CanRun())
        {
            isRun = false;
            return;
        }

        bool runKey = Input.GetKey(KeyCode.LeftShift);
        bool hasMoveInput = moveInput.sqrMagnitude > 0.0001f;

        isRun = runKey && hasMoveInput && !runLocked;
    }

    private void HandleDodgeInput()
    {
        if (!stateController.CanDodge())
            return;

        if (Input.GetButtonDown("Jump") && Time.time >= lastDodgeTime + dodgeCooldown)
        {
            if (playerStat != null && playerStat.TakeStamina(dodgeStaminaCost))
            {
                StartDodge();
            }
        }
    }

    private void HandleAttackInput()
    {
        if (!stateController.CanAttack())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }
    }

    // =========================================================
    // Movement Direction Layer
    // =========================================================
    private void UpdateMoveDirection()
    {
        if (cameraController == null)
        {
            moveDirection = moveInput;
            return;
        }

        Vector3 camForward = cameraController.GetCameraForwardOnPlane();
        Vector3 camRight = cameraController.GetCameraRightOnPlane();

        moveDirection = (camForward * moveInput.z + camRight * moveInput.x).normalized;
    }

    // =========================================================
    // Physics Motion Layer
    // =========================================================
    private void HandleMovementPhysics()
    {
        if (stateController.IsState(PlayerState.Dead))
            return;

        if (stateController.IsState(PlayerState.Hit))
            return;

        if (stateController.IsState(PlayerState.Dodge))
        {
            HandleDodgeMovement();
            return;
        }

        if (stateController.IsState(PlayerState.Attack))
            return;

        if (stateController.IsState(PlayerState.Guard))
            return;

        Move();
        Rotate();
    }

    private void HandleDodgeMovement()
    {
        rb.MovePosition(rb.position + dodgeDirection * dodgeSpeed * Time.fixedDeltaTime);
    }

    private void Move()
    {
        if (!stateController.CanMove())
            return;

        float currentSpeed = isRun ? runSpeed : walkSpeed;
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        if (!stateController.CanMove())
            return;

        if (moveDirection.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        Quaternion newRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation);
    }

    // =========================================================
    // Resource Layer
    // =========================================================
    private void HandleRunStamina()
    {
        if (!isRun) return;
        if (playerStat == null) return;

        bool success = playerStat.TakeStamina(runStaminaCostPerSecond * Time.deltaTime);

        if (!success)
        {
            isRun = false;
            runLocked = true;
        }
    }

    private void HandleGuardStamina()
    {
        if (!stateController.IsState(PlayerState.Guard)) return;
        if (playerStat == null) return;

        bool success = playerStat.TakeStamina(guardStaminaCostPerSecond * Time.deltaTime);

        if (!success)
        {
            guardLocked = true;
            EndGuard();
            Debug.Log("스태미너 부족 → 가드 해제");
        }
    }

    private void UpdateRunLock()
    {
        if (playerStat == null) return;

        float currentStamina = playerStat.GetCurrentStamina();

        if (runLocked && currentStamina >= minStaminaToStartRun)
        {
            runLocked = false;
        }
    }

    private void UpdateGuardLock()
    {
        if (playerStat == null) return;

        float currentStamina = playerStat.GetCurrentStamina();

        if (guardLocked && currentStamina >= minStaminaToStartGuard)
        {
            guardLocked = false;
        }
    }

    // =========================================================
    // Action Layer : Guard
    // =========================================================
    private void TryStartGuard()
    {
        if (playerStat == null) return;
        if (!stateController.CanGuard()) return;
        if (guardLocked) return;

        if (playerStat.GetCurrentStamina() < minStaminaToStartGuard)
            return;

        if (!stateController.ChangeState(PlayerState.Guard))
            return;

        guardEndTime = Time.time + guardDuration;
        playerStat.SetGuarding(true);
        isRun = false;

        if (animator != null)
        {
            animator.SetBool("isGuarding", true);
        }

        Debug.Log("가드 시작");
    }

    private void EndGuard()
    {
        if (stateController.IsState(PlayerState.Guard))
        {
            stateController.ChangeState(PlayerState.Idle);
        }

        if (playerStat != null)
        {
            playerStat.SetGuarding(false);
        }

        if (animator != null)
        {
            animator.SetBool("isGuarding", false);
        }

        Debug.Log("가드 종료");
    }

    // =========================================================
    // Action Layer : Dodge
    // =========================================================
    private void StartDodge()
    {
        if (stateController.IsState(PlayerState.Guard))
        {
            EndGuard();
        }

        if (!stateController.ChangeState(PlayerState.Dodge))
            return;

        if (playerStat != null)
        {
            playerStat.SetInvincible(true);
        }

        Vector3 baseDirection = moveDirection.sqrMagnitude > 0.0001f
            ? moveDirection
            : transform.forward;

        lastDodgeTime = Time.time;
        dodgeEndTime = Time.time + dodgeDuration;
        dodgeDirection = baseDirection.normalized;

        ResetMotionInput();
        StopRigidBodyMotion();

        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }
    }

    private void EndDodge()
    {
        if (stateController.IsState(PlayerState.Dodge))
        {
            stateController.ChangeState(PlayerState.Idle);
        }

        dodgeDirection = Vector3.zero;

        if (playerStat != null)
        {
            playerStat.SetInvincible(false);
        }
    }

    // =========================================================
    // Action Layer : Attack
    // =========================================================
    private void StartAttack()
    {
        if (playerStat == null) return;
        if (playerCombat == null) return;
        if (!stateController.CanAttack()) return;

        playerStat.SetInvincible(false);

        if (!playerStat.TakeStamina(attackStaminaCost))
        {
            Debug.Log("스태미너 부족 - 공격 불가");
            return;
        }

        if (!stateController.ChangeState(PlayerState.Attack))
            return;

        attackEndTime = Time.time + attackDuration;

        ResetMotionInput();
        StopRigidBodyMotion();

        playerCombat.StartAttack();

        if (animator != null)
        {
            animator.SetBool("isGuarding", false);
            animator.SetTrigger("Attack");
        }
    }

    private void EndAttack()
    {
        playerCombat?.EndAttack();

        if (stateController.IsState(PlayerState.Attack))
        {
            stateController.ChangeState(PlayerState.Idle);
        }
    }

    public void NotifyAttackAnimationEnded()
    {
        if (stateController == null)
            return;

        if (stateController.IsState(PlayerState.Attack))
        {
            EndAttack();
        }
    }

    // =========================================================
    // Action Layer : HitStun
    // =========================================================
    public void StartHitStun()
    {
        StartHitStun(hitStunDuration);
    }

    public void StartHitStun(float duration)
    {
        if (stateController.IsState(PlayerState.Dead))
            return;

        if (stateController.IsState(PlayerState.Guard))
        {
            EndGuard();
        }

        if (stateController.IsState(PlayerState.Attack))
        {
            playerCombat?.CancelAttack();
        }

        stateController.ChangeState(PlayerState.Hit);

        isRun = false;
        hitStunEndTime = Time.time + duration;

        ResetMotionInput();
        dodgeDirection = Vector3.zero;
        StopRigidBodyMotion();
    }

    private void EndHitStun()
    {
        if (stateController.IsState(PlayerState.Hit))
        {
            stateController.ChangeState(PlayerState.Idle);
        }
    }

    public bool IsHitStunned()
    {
        return stateController != null && stateController.IsState(PlayerState.Hit);
    }

    // =========================================================
    // Locomotion State Layer
    // - Idle / Move / Run 상태 갱신
    // =========================================================
    private void UpdateLocomotionState()
    {
        if (stateController == null)
            return;

        if (stateController.IsInActionState())
            return;

        if (moveDirection.sqrMagnitude < 0.0001f)
        {
            stateController.ChangeState(PlayerState.Idle);
            return;
        }

        if (isRun)
        {
            stateController.ChangeState(PlayerState.Run);
        }
        else
        {
            stateController.ChangeState(PlayerState.Move);
        }
    }

    // =========================================================
    // Animation Layer
    // =========================================================
    private void UpdateAnimation()
    {
        if (animator == null || stateController == null)
            return;

        if (stateController.IsState(PlayerState.Dodge) ||
            stateController.IsState(PlayerState.Attack) ||
            stateController.IsState(PlayerState.Hit))
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float animSpeed = 0f;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            animSpeed = stateController.IsState(PlayerState.Run) ? 1f : 0.4f;
        }

        animator.SetFloat("Speed", animSpeed);
    }

    // =========================================================
    // Utility Layer
    // =========================================================
    private void ResetMotionInput()
    {
        moveInput = Vector3.zero;
        moveDirection = Vector3.zero;
        isRun = false;
    }

    private void StopRigidBodyMotion()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}