using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Detect")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;

    [Header("Hit Reaction")]
    [SerializeField] private float hitStunDuration = 0.3f;
    [SerializeField] private float knockbackDistance = 1.0f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private NavMeshAgent agent;
    private MonsterStat monsterStat;
    private Animator animator;

    private float lastAttackTime = -999f;
    private bool isAttacking = false;

    private bool isHitStunned = false;
    private float hitStunEndTime = -999f;

    private bool isKnockback = false;
    private float knockbackEndTime = -999f;
    private Vector3 knockbackDirection = Vector3.zero;
    private float knockbackSpeed = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterStat = GetComponent<MonsterStat>();
        animator = GetComponentInChildren<Animator>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target == null)
        {
            Debug.LogError("MonsterAI: Player target을 찾지 못했습니다.");
        }

        if (animator == null)
        {
            Debug.LogError("MonsterAI: Animator를 찾지 못했습니다.");
        }
    }

    private void Update()
    {
        if (target == null) return;

        if (monsterStat != null && monsterStat.IsDead())
        {
            StopMoving();
            UpdateAnimation();
            return;
        }

        HandleHitReaction();

        if (isHitStunned || isKnockback)
        {
            StopMoving();
            UpdateAnimation();
            return;
        }

        if (isAttacking)
        {
            StopMoving();
            UpdateAnimation();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            StopMoving();
            FaceTarget();

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                isAttacking = true;

                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
        else if (distance <= detectRange)
        {
            ChaseTarget();
        }
        else
        {
            StopMoving();
        }

        UpdateAnimation();
    }

    private void HandleHitReaction()
    {
        if (isKnockback)
        {
            Vector3 move = knockbackDirection * knockbackSpeed * Time.deltaTime;

            // 🔥 핵심: NavMeshAgent로 이동
            if (agent != null && agent.isOnNavMesh)
            {
                agent.Move(move);
            }
            else
            {
                transform.position += move;
            }

            if (Time.time >= knockbackEndTime)
            {
                isKnockback = false;
            }
        }

        if (isHitStunned && Time.time >= hitStunEndTime)
        {
            isHitStunned = false;
        }
    }

    private void ChaseTarget()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    private void StopMoving()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void FaceTarget()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            8f * Time.deltaTime
        );
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        if (!agent.isOnNavMesh || isAttacking || isHitStunned || isKnockback)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float speed = agent.velocity.magnitude;
        float animSpeed = 0f;

        if (agent.speed > 0.001f)
        {
            animSpeed = speed / agent.speed;
        }

        animator.SetFloat("Speed", animSpeed);
    }

    public void StartAttack()
    {
        if (isHitStunned || isKnockback) return;

        isAttacking = true;
        StopMoving();
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void StartHitReaction(Transform attacker)
    {
        if (monsterStat != null && monsterStat.IsDead()) return;

        isAttacking = false;
        isHitStunned = true;
        hitStunEndTime = Time.time + hitStunDuration;

        if (attacker != null)
        {
            Vector3 dir = transform.position - attacker.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                knockbackDirection = dir.normalized;
                knockbackSpeed = knockbackDistance / knockbackDuration;
                isKnockback = true;
                knockbackEndTime = Time.time + knockbackDuration;
            }
        }

        StopMoving();
    }

    public bool IsHitStunned()
    {
        return isHitStunned || isKnockback;
    }
}