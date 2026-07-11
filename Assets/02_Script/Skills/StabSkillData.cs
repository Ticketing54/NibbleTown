using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "StabSkill", menuName = "NibbleTown/Skills/Stab")]
public class StabSkillData : SkillData
{
    [Header("타게팅")]
    public float targetRange = 8f;

    [Header("돌진")]
    public float windup       = 0.1f;
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;

    [Header("밀치기")]
    public float pushForce  = 15f;
    public float pushRadius = 1.2f;

    [Header("데미지")]
    public float damageMultiplier = 2f;

    public override IEnumerator Execute(SkillContext ctx)
    {
        var       cc     = ctx.caster.GetComponent<CharacterController>();
        Transform caster = ctx.caster.transform;

        ctx.skillAnimator?.Play(skillClip);

        Vector3 dashDir = FindTargetDirection(caster, ctx.targetLayer);
        caster.rotation = Quaternion.LookRotation(dashDir);

        yield return new WaitForSeconds(windup);

        float elapsed = 0f;
        float speed   = dashDistance / dashDuration;
        var   damaged = new HashSet<IDamageable>();

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            cc?.Move(dashDir * speed * Time.deltaTime);

            Collider[] hits = Physics.OverlapSphere(caster.position, pushRadius, ctx.targetLayer);
            foreach (Collider col in hits)
            {
                var agent = col.GetComponentInParent<NavMeshAgent>();
                if (agent != null && agent.enabled)
                    agent.Warp(agent.transform.position + dashDir * pushForce * Time.deltaTime);

                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || damageable.IsDead || !damaged.Add(damageable)) continue;

                ApplyDamage(ctx, damageable);
            }

            yield return null;
        }
    }

    private void ApplyDamage(SkillContext ctx, IDamageable target)
    {
        CharacterStatConfig config = ctx.stat.Config;
        if (config == null) return;

        int   baseDamage = Mathf.RoundToInt(config.attackDamage * damageMultiplier);
        bool  isCrit     = Random.value < config.critChance;
        float variance   = Random.Range(0.8f, 1.1f);
        float multiplier = isCrit ? variance + config.critBonusRate : variance;
        int   final      = Mathf.RoundToInt(baseDamage * multiplier);

        target.TakeDamage(new DamageInfo(ctx.caster, final, isCrit));
    }

    private Vector3 FindTargetDirection(Transform caster, LayerMask targetLayer)
    {
        Collider[] hits = Physics.OverlapSphere(caster.position, targetRange, targetLayer);
        if (hits.Length == 0) return caster.forward;

        Transform facing         = null;
        Transform nearest        = null;
        float     minFacingDist  = float.MaxValue;
        float     minNearestDist = float.MaxValue;

        foreach (Collider col in hits)
        {
            Vector3 toTarget = col.transform.position - caster.position;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            if (dist < minNearestDist)
            {
                minNearestDist = dist;
                nearest        = col.transform;
            }

            if (Vector3.Angle(caster.forward, toTarget) < 60f && dist < minFacingDist)
            {
                minFacingDist = dist;
                facing        = col.transform;
            }
        }

        Transform target = facing != null ? facing : nearest;
        Vector3   dir    = target.position - caster.position;
        dir.y = 0f;
        return dir.normalized;
    }
}
