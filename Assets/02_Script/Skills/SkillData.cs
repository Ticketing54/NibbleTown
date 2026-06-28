using System.Collections;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    public int    skillId;
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;
    public int           price;
    public int           mpCost;
    public float         cooldown;
    public AnimationClip skillClip;

    // Execute() 중 이동을 잠글지 여부
    public virtual bool LockMovementDuringExecute    => true;
    // Execute() 완료 후 애니메이션 종료까지 대기할지 여부
    public virtual bool WaitForAnimationAfterExecute => true;

    public abstract IEnumerator Execute(SkillContext ctx);
}
