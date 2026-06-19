using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    public int    skillId;
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;
    public int    mpCost;
    public float  cooldown;

    public abstract void Execute(SkillContext ctx);
}
