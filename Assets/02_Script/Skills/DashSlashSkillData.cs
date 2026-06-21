using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "DashSlashSkill", menuName = "NibbleTown/Skills/Dash Slash")]
public class DashSlashSkillData : SkillData
{
    [Header("타게팅")]
    public float targetRange = 8f;

    [Header("돌진")]
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;

    [Header("밀기")]
    public float pushForce  = 10f;
    public float pushRadius = 1.2f;

    [Header("베기")]
    public float slashDamageMultiplier = 2.5f;
    public float slashRadius           = 2f;
    [Range(0f, 180f)]
    public float slashAngle            = 120f;

    public override IEnumerator Execute(SkillContext ctx)
    {
        var cc     = ctx.caster.GetComponent<CharacterController>();
        Transform caster = ctx.caster.transform;

        ctx.skillAnimator?.Play(skillClip);

        Vector3 dashDir = FindTargetDirection(caster, ctx.targetLayer);
        caster.rotation = Quaternion.LookRotation(dashDir);

        float speed   = dashDistance / dashDuration;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            cc?.Move(dashDir * speed * Time.deltaTime);

            Collider[] pushHits = Physics.OverlapSphere(caster.position, pushRadius, ctx.targetLayer);
            foreach (Collider col in pushHits)
            {
                var agent = col.GetComponentInParent<NavMeshAgent>();
                if (agent != null && agent.enabled)
                    agent.Warp(agent.transform.position + dashDir * pushForce * Time.deltaTime);
            }

            yield return null;
        }

        ApplySlash(caster, ctx);
    }

    private Vector3 FindTargetDirection(Transform caster, LayerMask targetLayer)
    {
        Collider[] hits = Physics.OverlapSphere(caster.position, targetRange, targetLayer);
        if (hits.Length == 0) return caster.forward;

        Transform facing  = null;
        Transform nearest = null;
        float minFacingDist  = float.MaxValue;
        float minNearestDist = float.MaxValue;

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

    private void ApplySlash(Transform caster, SkillContext ctx)
    {
        CharacterStatConfig config = ctx.stat.Config;
        if (config == null) return;

        int baseDamage = Mathf.RoundToInt(config.attackDamage * slashDamageMultiplier);

        Collider[] hits = Physics.OverlapSphere(caster.position, slashRadius, ctx.targetLayer);
        foreach (Collider col in hits)
        {
            Vector3 toTarget = col.transform.position - caster.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.001f) continue;
            if (Vector3.Angle(caster.forward, toTarget) > slashAngle * 0.5f) continue;

            var damageable = col.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead) continue;

            bool  isCrit     = Random.value < config.critChance;
            float variance   = Random.Range(0.8f, 1.1f);
            float multiplier = isCrit ? variance + config.critBonusRate : variance;
            int   final      = Mathf.RoundToInt(baseDamage * multiplier);

            damageable.TakeDamage(new DamageInfo(ctx.caster, final, isCrit));
        }
    }
}
