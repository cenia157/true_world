using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCam;
    private Animator animator;

    public float speed = 5f;
    public float runSpeed = 10f;
    public float rotSpeed = 16f;

    public bool isRun = false;

    public float dashSpeed = 10f;
    public float dashCooldown = 2f;
    public float dashDuration = 1.25f;

    public float attackDuration = 1.0f;

    private Vector3 dir = Vector3.zero;

    private bool isDashing = false;
    private float lastDashTime = -999f;
    private float dashEndTime = 0f;
    private Vector3 dashDirection = Vector3.zero;

    private bool isAttacking = false;
    private float attackEndTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator를 찾지 못했습니다.");
        }
    }

    void Update()
    {
        if (!isDashing && !isAttacking)
        {
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.z = Input.GetAxisRaw("Vertical");
            dir.Normalize();

            isRun = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetButtonDown("Jump") && Time.time >= lastDashTime + dashCooldown)
            {
                StartDash();
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartAttack();
            }
        }

        if (isDashing && Time.time >= dashEndTime)
        {
            EndDash();
        }

        if (isAttacking && Time.time >= attackEndTime)
        {
            EndAttack();
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            return;
        }

        if (isAttacking)
        {
            return;
        }

        RotateToMouse();

        float currentSpeed = isRun ? runSpeed : speed;
        rb.MovePosition(rb.position + dir * currentSpeed * Time.fixedDeltaTime);
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (isDashing || isAttacking)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float animSpeed = 0f;

        if (dir.magnitude > 0.1f)
        {
            animSpeed = isRun ? 1f : 0.4f;
        }

        animator.SetFloat("Speed", animSpeed);
    }

    void StartDash()
    {
        Vector3 lookDir = GetMouseDirection();

        if (lookDir == Vector3.zero)
            return;

        isDashing = true;
        lastDashTime = Time.time;
        dashEndTime = Time.time + dashDuration;
        dashDirection = lookDir.normalized;

        dir = Vector3.zero;
        isRun = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }
    }

    void EndDash()
    {
        isDashing = false;
        dashDirection = Vector3.zero;
    }

    void StartAttack()
    {
        isAttacking = true;
        attackEndTime = Time.time + attackDuration;

        dir = Vector3.zero;
        isRun = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    Vector3 GetMouseDirection()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            Vector3 lookDir = mouseWorldPos - transform.position;
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
                return lookDir.normalized;
        }

        return Vector3.zero;
    }

    void RotateToMouse()
    {
        if (isDashing || isAttacking)
            return;

        Vector3 lookDir = GetMouseDirection();

        if (lookDir != Vector3.zero)
        {
            Vector3 newDir = Vector3.Lerp(
                transform.forward,
                lookDir,
                rotSpeed * Time.fixedDeltaTime
            );

            transform.forward = newDir;
        }
    }
}