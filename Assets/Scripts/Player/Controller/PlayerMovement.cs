using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private PlayerCameraController cameraController;

    private Vector3 moveInput;
    private Vector3 moveDirection;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 16f;

    [Header("Dodge")]
    [SerializeField] private float dodgeCooldown = 2f;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float dodgeDuration = 0.5f;

    private bool isRun;
    private bool isDodge;
    private Vector3 dodgeDirection;
    private float lastDodgeTime = -999f;
    private float dodgeEndTime;

    [Header("Attack")]
    [SerializeField] private float attackDuration = 1.0f;

    private bool isAttacking;
    private float attackEndTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        cameraController = Camera.main.GetComponent<PlayerCameraController>();

        if (animator == null)
        {
            Debug.LogError("Animator를 찾지 못했습니다.");
        }

        if (cameraController == null)
        {
            Debug.LogError("Main Camera에 PlayerCameraController가 없습니다.");
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
        if (isDodge)
        {
            rb.MovePosition(rb.position + dodgeDirection * dodgeSpeed * Time.fixedDeltaTime);
            return;
        }

        if (isAttacking)
        {
            return;
        }

        Move();
        Rotate();
    }

    private void HandleInput()
    {
        if (isDodge || isAttacking)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(x, 0f, z).normalized;
        isRun = Input.GetKey(KeyCode.LeftShift);

        UpdateMoveDirection();

        if (Input.GetButtonDown("Jump") && Time.time >= lastDodgeTime + dodgeCooldown)
        {
            StartDodge();
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartAttack();
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
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackEndTime = Time.time + attackDuration;

        moveInput = Vector3.zero;
        moveDirection = Vector3.zero;
        isRun = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (isDodge || isAttacking)
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