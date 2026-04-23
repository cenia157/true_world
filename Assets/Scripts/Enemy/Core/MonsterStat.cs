using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterDamageHandler))]
public class MonsterStat : CharacterStat
{
    private MonsterDamageHandler damageHandler;

    protected override void Start()
    {
        base.Start();

        damageHandler = GetComponent<MonsterDamageHandler>();

        if (damageHandler == null)
        {
            Debug.LogError("MonsterStat: MonsterDamageHandler를 찾지 못했습니다.");
        }
    }

    public override void TakePhysicalDamage(int damage, Transform attacker)
    {
        if (damageHandler == null)
        {
            Debug.LogWarning("MonsterDamageHandler가 없어 기본 데미지 처리를 수행합니다.");
            base.TakePhysicalDamage(damage, attacker);
            return;
        }

        damageHandler.ReceivePhysicalDamage(damage, attacker);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log($"{gameObject.name} 사망");

        MonsterAI ai = GetComponent<MonsterAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        MonsterCombat combat = GetComponent<MonsterCombat>();
        if (combat != null)
        {
            combat.CancelAttack();
            combat.enabled = false;
        }

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.enabled = false;
        }

        SetDeadMaterial();

        CorpseLoot corpseLoot = GetComponent<CorpseLoot>();
        if (corpseLoot != null)
        {
            corpseLoot.GenerateLoot();
        }
    }

    private void SetDeadMaterial()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.color = Color.gray;
            }
        }
    }
}