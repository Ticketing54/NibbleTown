using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "DashSkill", menuName = "NibbleTown/Skills/Dash")]
public class DashSkillData : SkillData
{
    [Header("돌진")]
    public float dashDistance = 6f;
    public float dashDuration = 0.25f;

    [Header("밀기")]
    public float pushForce  = 10f;
    public float pushRadius = 1.2f;

    public override IEnumerator Execute(SkillContext ctx)
    {
        var cc     = ctx.caster.GetComponent<CharacterController>();
        Transform caster = ctx.caster.transform;

        ctx.skillAnimator?.Play(skillClip);

        Vector3 dashDir = caster.forward;
        float   speed   = dashDistance / dashDuration;
        float   elapsed = 0f;

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
            }

            yield return null;
        }
    }
}
