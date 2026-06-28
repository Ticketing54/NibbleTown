using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBuffSkill", menuName = "NibbleTown/Skills/SpeedBuff")]
public class SpeedBuffSkillData : SkillData
{
    [Header("버프 수치")]
    [Tooltip("이동속도 배율 (1.5 = 50% 증가)")]
    public float moveSpeedMultiplier   = 1.5f;
    [Tooltip("공격속도 배율 (1.5 = 50% 증가)")]
    public float attackSpeedMultiplier = 1.5f;
    public float duration              = 5f;

    private Coroutine activeTimer;

    public override IEnumerator Execute(SkillContext ctx)
    {
        ctx.skillAnimator?.Play(skillClip);

        var controller = ctx.caster.GetComponent<PlayerController>();
        var attack     = ctx.caster.GetComponent<PlayerAttack>();
        var executor   = ctx.caster.GetComponent<SkillExecutor>();

        if (activeTimer != null && executor != null)
            executor.StopCoroutine(activeTimer);

        controller?.SetSpeedMultiplier(moveSpeedMultiplier);
        attack?.SetAttackSpeedMultiplier(attackSpeedMultiplier);

        if (executor != null)
            activeTimer = executor.StartCoroutine(BuffTimer(controller, attack));

        yield break;
    }

    private IEnumerator BuffTimer(PlayerController controller, PlayerAttack attack)
    {
        yield return new WaitForSeconds(duration);
        controller?.SetSpeedMultiplier(1f);
        attack?.SetAttackSpeedMultiplier(1f);
        activeTimer = null;
    }
}
