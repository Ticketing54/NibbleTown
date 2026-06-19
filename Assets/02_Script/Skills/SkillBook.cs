using System.Collections.Generic;
using UnityEngine;

public class SkillBook : MonoBehaviour
{
    private readonly SkillData[] slots = new SkillData[3];

    public IReadOnlyList<SkillData> Slots => slots;
    public bool IsFull => FindEmptySlot() < 0;

    // 상점에서 구매 시 호출 — 빈 슬롯에 자동 배치
    public bool AddSkill(int _skillId)
    {
        int slot = FindEmptySlot();
        if (slot < 0) return false;
        return SetSlot(slot, _skillId);
    }

    // 특정 슬롯에 직접 배치 (슬롯 UI에서 교체 시 사용)
    public bool SetSlot(int _slotIndex, int _skillId)
    {
        if ((uint)_slotIndex >= 3) return false;

        var skill = GameDataManager.Skills.Get(_skillId);
        if (skill == null) return false;

        slots[_slotIndex] = skill;
        GameEvents.RaiseSkillEquipped(_slotIndex, _skillId);
        return true;
    }

    // 슬롯 비우기
    public void RemoveSkill(int _slotIndex)
    {
        if ((uint)_slotIndex >= 3) return;
        slots[_slotIndex] = null;
        GameEvents.RaiseSkillEquipped(_slotIndex, -1);
    }

    // 두 슬롯 간 스킬 교체
    public void SwapSlots(int _from, int _to)
    {
        if ((uint)_from >= 3 || (uint)_to >= 3) return;

        (slots[_from], slots[_to]) = (slots[_to], slots[_from]);
        GameEvents.RaiseSkillEquipped(_from, slots[_from]?.skillId ?? -1);
        GameEvents.RaiseSkillEquipped(_to,   slots[_to]?.skillId   ?? -1);
    }

    public SkillData GetEquipped(int _slotIndex)
    {
        if ((uint)_slotIndex >= 3) return null;
        return slots[_slotIndex];
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] == null) return i;
        return -1;
    }
}
