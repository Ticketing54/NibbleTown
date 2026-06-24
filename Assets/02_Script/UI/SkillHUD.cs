using UnityEngine;

public class SkillHUD : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] slots = new SkillSlotUI[3];

    private void OnEnable()
    {
        GameEvents.OnSkillEquipped     += OnSkillEquipped;
        GameEvents.OnSkillCooldownTick += OnSkillCooldownTick;
    }

    private void OnDisable()
    {
        GameEvents.OnSkillEquipped     -= OnSkillEquipped;
        GameEvents.OnSkillCooldownTick -= OnSkillCooldownTick;
    }

    private void OnSkillEquipped(int slotIndex, int skillId)
    {
        if ((uint)slotIndex >= slots.Length || slots[slotIndex] == null) return;
        SkillData skill = skillId >= 0 ? GameDataManager.Skills.Get(skillId) : null;
        slots[slotIndex].SetSkill(skill);
    }

    private void OnSkillCooldownTick(int slotIndex, float ratio, float remaining)
    {
        if ((uint)slotIndex >= slots.Length || slots[slotIndex] == null) return;
        slots[slotIndex].UpdateCooldown(ratio, remaining);
    }
}
