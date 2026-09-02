using System;

public static partial class GameEvents
{
    // ── 스킬 ─────────────────────────────────────────────────────
    public static event Action<int, int>          OnSkillEquipped;     // slotIndex, skillId (-1 = 해제)
    public static event Action<int>               OnSkillUsed;         // slotIndex
    public static event Action<int, float, float> OnSkillCooldownTick; // slotIndex, ratio, remaining

    public static void RaiseSkillEquipped(int _slotIndex, int _skillId)                       => OnSkillEquipped?.Invoke(_slotIndex, _skillId);
    public static void RaiseSkillUsed(int _slotIndex)                                         => OnSkillUsed?.Invoke(_slotIndex);
    public static void RaiseSkillCooldownTick(int _slotIndex, float _ratio, float _remaining) => OnSkillCooldownTick?.Invoke(_slotIndex, _ratio, _remaining);
}
