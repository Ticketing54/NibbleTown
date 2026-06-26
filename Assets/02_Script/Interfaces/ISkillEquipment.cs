public interface ISkillEquipment
{
    bool      SetSlot(int slotIndex, int skillId);
    SkillData GetEquipped(int slotIndex);
}
