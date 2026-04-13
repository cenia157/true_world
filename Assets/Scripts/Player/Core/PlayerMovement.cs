using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private PlayerCameraController cameraController;
    private PlayerStat playerStat;

    private Vector3 moveInput;
    private Vector3 moveDirection;
    private Vector3 dodgeDirection;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 16f;

    private bool isRun;
    private bool runLocked = false;
    [Header("Run Stamina")]
    [SerializeField] private float runStaminaCostPerSecond = 25f;
    [SerializeField] private float minStaminaToStartRun = 20f;

    private bool isDodge;
    private float lastDodgeTime = -999f;
    private float dodgeEndTime;
    [Header("Dodge")]
    [SerializeField] private float dodgeCooldown = 0.5f;
    [SerializeField] private float dodgeSpeed = 6f;
    [SerializeField] private float dodgeDuration = 1.25f;
    [SerializeField] private float dodgeStaminaCost = 30f;

    private bool isAttacking;
    private float attackEndTime;
    [Header("Attack")]
    [SerializeField] private float attackDuration = 1.0f;
    [SerializeField] private float attackStaminaCost = 15f;

    private bool isGuarding;
    private bool guardLocked = false;
    private float guardEndTime;
    [Header("Guard")]
    [SerializeField] private float guardStaminaCostPerSecond = 50f;
    [SerializeField] private float guardDuration = 1.0f;
    [SerializeField] private float minStaminaToStartGuard = 25f;

    [Header("HitStun")]
    [SerializeField] private float hitStunDuration = 0.5f;

    private bool isHitStunned = false;
    private float hitStunEndTime = -999f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        cameraController = Camera.main.GetComponent<PlayerCameraController>();
        playerStat = GetComponent<PlayerStat>();

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
    }

    private void Update()
    {
        HandleStateTimer();
        HandleInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isHitStunned)
        {
            return;
        }

        if (isDodge)
        {
            rb.MovePosition(rb.position + dodgeDirection * dodgeSpeed * Time.fixedDeltaTime);
            return;
        }

        if (isAttacking)
        {
            return;
        }

        if (isGuarding)
        {
            return;
        }

        Move();
        Rotate();
    }

    private void HandleInput()
    {
        if (isHitStunned)
            return;

        UpdateGuardLock();

        if (Input.GetMouseButtonDown(1))
        {
            TryStartGuard();
        }

        if (isDodge || isAttacking)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(x, 0f, z).normalized;

        bool runKey = Input.GetKey(KeyCode.LeftShift);
        bool hasMoveInput = moveInput.sqrMagnitude > 0.0001f;

        UpdateMoveDirection();
        UpdateRunLock();

        isRun = runKey && hasMoveInput && !runLocked && !isGuarding;

        HandleRunStamina();
        HandleGuardStamina();

        if (isGuarding)
            return;

        if (Input.GetButtonDown("Jump") && Time.time >= lastDodgeTime + dodgeCooldown)
        {
            if (playerStat != null && playerStat.TakeStamina(dodgeStaminaCost))
            {
                StartDodge();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }
    }

    private void TryStartGuard()
    {
        if (playerStat == null) return;
        if (isGuarding) return;
        if (isDodge || isAttacking || isHitStunned) return;
        if (guardLocked) return;

        if (playerStat.GetCurrentStamina() < minStaminaToStartGuard)
            return;

        isGuarding = true;
        guardEndTime = Time.time + guardDuration;

        playerStat.SetGuarding(true);

        if (animator != null)
        {
            animator.SetBool("isGuarding", true);
        }

        Debug.Log("가드 시작");
    }

    private void EndGuard()
    {
        isGuarding = false;

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

    private void UpdateRunLock()
    {
        if (playerStat == null) return;

        float currentStamina = playerStat.GetCurrentStamina();

        if (runLocked && currentStamina >= minStaminaToStartRun)
        {
            runLocked = false;
        }
    }

    private void HandleStateTimer()
    {
        if (isDodge && Time.time >= dodgeEndTime)
        {
            EndDodge();
        }

        if (isAttacking && Time.time >= attackEndTime)
        {
            EndAttack();
        }

        if (isGuarding && Time.time >= guardEndTime)
        {
            EndGuard();
        }

        if (isHitStunned && Time.time >= hitStunEndTime)
        {
            EndHitStun();
        }
    }

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

    private void Move()
    {
        float currentSpeed = isRun ? runSpeed : walkSpeed;
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
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

    private void StartDodge()
    {
        if (isGuarding)
        {
            EndGuard();
        }

        if (playerStat != null)
        {
            playerStat.SetInvincible(true);
        }

        Vector3 baseDirection = moveDirection.sqrMagnitude > 0.0001f ? moveDirection : transform.forward;

        isDodge = true;
        lastDodgeTime = Time.time;
        dodgeEndTime = Time.time + dodgeDuration;
        dodgeDirection = baseDirection.normalized;

        moveInput = Vector3.zero;
        moveDirection = Vector3.zero;
        isRun = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }
    }

    private void EndDodge()
    {
        isDodge = false;
        dodgeDirection = Vector3.zero;

        if (playerStat != null)
        {
            playerStat.SetInvincible(false);
        }
    }

    private void StartAttack()
    {
        if (playerStat == null) return;
        if (isGuarding || isHitStunned) return;

        playerStat.SetInvincible(false);

        if (!playerStat.TakeStamina(attackStaminaCost))
        {
            Debug.Log("스태미너 부족 - 공격 불가");
            return;
        }

        isAttacking = true;
        attackEndTime = Time.time + attackDuration;

        moveInput = Vector3.zero;
        moveDirection = Vector3.zero;
        isRun = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetBool("isGuarding", false);
            animator.SetTrigger("Attack");
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private void HandleGuardStamina()
    {
        if (!isGuarding) return;
        if (playerStat == null) return;

        bool success = playerStat.TakeStamina(guardStaminaCostPerSecond * Time.deltaTime);

        if (!success)
        {
            guardLocked = true;
            EndGuard();

            Debug.Log("스태미너 부족 → 가드 해제");
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

    public void StartHitStun()
    {
        StartHitStun(hitStunDuration);
    }

    public void StartHitStun(float duration)
    {
        if (isDeadLikeState())
            return;

        if (isGuarding)
        {
            EndGuard();
        }

        isRun = false;
        isAttacking = false;
        isHitStunned = true;
        hitStunEndTime = Time.time + duration;

        moveInput = Vector3.zero;
        moveDirection = Vector3.zero;
        dodgeDirection = Vector3.zero;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void EndHitStun()
    {
        isHitStunned = false;
    }

    public bool IsHitStunned()
    {
        return isHitStunned;
    }

    private bool isDeadLikeState()
    {
        return isDodge;
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (isDodge || isAttacking || isHitStunned)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float animSpeed = 0f;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            animSpeed = isRun ? 1f : 0.4f;
        }

        animator.SetFloat("Speed", animSpeed);
    }
}