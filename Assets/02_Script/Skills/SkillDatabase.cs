using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "NibbleTown/Database/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private List<SkillData> entries = new();

    private Dictionary<int, SkillData> lookup;

    public IReadOnlyList<SkillData> All => entries;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, SkillData>(entries.Count);
        foreach (var e in entries)
            lookup[e.skillId] = e;
    }

    public SkillData Get(int _skillId)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(_skillId, out var data) ? data : null;
    }
}
