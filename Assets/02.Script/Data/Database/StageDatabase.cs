using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageData
{
    public int    stageId;
    public string stageName;
    public int    requiredLevel;
    public string monsterType;
    public int    monsterCount;
    public string bossMonsterType;
    public int    timeLimitSec;
    public int    rewardItem1Id;
    public int    rewardItem1Count;
    public int    rewardItem2Id;
    public int    rewardItem2Count;
    public int    rewardExp;
    public int    rewardGold;
}

[CreateAssetMenu(fileName = "StageDatabase", menuName = "NibbleTown/Database/Stage Database")]
public class StageDatabase : ScriptableObject
{
    [SerializeField] private List<StageData> entries = new List<StageData>();

    private Dictionary<int, StageData> lookup;

    public IReadOnlyList<StageData> All => entries;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, StageData>(entries.Count);
        foreach (var e in entries)
            lookup[e.stageId] = e;
    }

    public StageData Get(int _stageId)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(_stageId, out var data) ? data : null;
    }
}
