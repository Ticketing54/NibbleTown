using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "JudgmentSkill", menuName = "NibbleTown/Skills/Judgment")]
public class JudgmentSkillData : SkillData
{
    [Header("회전 참격")]
    [Tooltip("회전 지속 시간(초)")]
    public float spinDuration = 3f;
    [Tooltip("데미지 범위 반경")]
    public float spinRadius   = 2f;
    [Tooltip("기본 공격력 대비 타격당 데미지 비율")]
    [Range(0f, 2f)] public float damageRatio = 0.5f;
    [Tooltip("타격 간격(초)")]
    public float tickInterval = 0.5f;
    [Tooltip("마지막 타격 데미지 배율")]
    public float finalTickBonusMultiplier = 2f;
    [Tooltip("회전 중 이동속도 배율 (1 = 정상)")]
    [Range(0f, 1f)] public float movementMultiplierDuringSpin = 0.7f;

    public override bool LockMovementDuringExecute    => false;
    public override bool WaitForAnimationAfterExecute => false;

    public override IEnumerator Execute(SkillContext ctx)
    {
        ctx.skillAnimator?.PlayLooping(skillClip);

        var  controller = ctx.caster.GetComponent<PlayerController>();
        var  casterTrans = ctx.caster.transform;
        int  baseDamage  = Mathf.RoundToInt(ctx.stat.Config.attackDamage * damageRatio);
        float critChance    = ctx.stat.Config.critChance;
        float critBonusRate = ctx.stat.Config.critBonusRate;

        controller?.SetSpeedMultiplier(movementMultiplierDuringSpin);

        float elapsed   = 0f;
        float nextTick  = 0f;
        int   totalTicks = Mathf.Max(1, Mathf.FloorToInt(spinDuration / tickInterval));
        int   ticksFired = 0;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= nextTick)
            {
                nextTick += tickInterval;
                ticksFired++;

                bool  isFinalTick = ticksFired >= totalTicks;
                float bonusMult   = isFinalTick ? finalTickBonusMultiplier : 1f;
                DealSpinDamage(ctx.caster, casterTrans.position, baseDamage, bonusMult,
                               critChance, critBonusRate, ctx.targetLayer);
            }

            yield return null;
        }

        ctx.skillAnimator?.StopLooping();
        controller?.SetSpeedMultiplier(1f);
    }

    private void DealSpinDamage(GameObject dealer, Vector3 center, int baseDamage,
                                float bonusMult, float critChance, float critBonusRate,
                                LayerMask targetLayer)
    {
        Collider[] hits = Physics.OverlapSphere(center, spinRadius, targetLayer);
        foreach (Collider col in hits)
        {
            var target = col.GetComponentInParent<IDamageable>();
            if (target == null || target.IsDead) continue;

            bool  isCrit     = Random.value < critChance;
            float variance   = Random.Range(0.8f, 1.1f);
            float multiplier = (isCrit ? variance + critBonusRate : variance) * bonusMult;
            int   finalDmg   = Mathf.RoundToInt(baseDamage * multiplier);

            target.TakeDamage(new DamageInfo(dealer, finalDmg, isCrit));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        // 씬 뷰에서 범위 미리보기 — 캐스터 위치 기준으로 그릴 수 없어 원점 기준
    }
}
