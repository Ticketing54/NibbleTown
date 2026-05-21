using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharStatData
{
    public int    characterId;
    public string characterName;
    public string characterClass;
    public int    baseHP;
    public int    baseMana;
    public int    baseAttack;
    public int    baseDefense;
    public float  baseSpeed;
    public float  baseAttackRange;
    public float  baseCritRate;
    public float  baseCritDamage;
    public int    hpPerLevel;
    public int    attackPerLevel;
    public int    defensePerLevel;
}

[CreateAssetMenu(fileName = "CharStatDatabase", menuName = "NibbleTown/Database/Char Stat Database")]
public class CharStatDatabase : ScriptableObject
{
    [SerializeField] private List<CharStatData> entries = new List<CharStatData>();

    private Dictionary<int, CharStatData> lookup;

    public IReadOnlyList<CharStatData> All => entries;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, CharStatData>(entries.Count);
        foreach (var e in entries)
            lookup[e.characterId] = e;
    }

    public CharStatData Get(int _characterId)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(_characterId, out var data) ? data : null;
    }
}
