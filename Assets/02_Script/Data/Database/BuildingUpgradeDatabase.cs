using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingUpgradeData
{
    public int    buildingId;
    public string buildingName;
    public int    currentLevel;
    public int    nextLevel;
    public int    requiredItem1Id;
    public int    requiredItem1Count;
    public int    requiredItem2Id;
    public int    requiredItem2Count;
    public int    requiredItem3Id;
    public int    requiredItem3Count;
    public int    requiredGold;
    public int    upgradeTimeSec;
}

[CreateAssetMenu(fileName = "BuildingUpgradeDatabase", menuName = "NibbleTown/Database/Building Upgrade Database")]
public class BuildingUpgradeDatabase : ScriptableObject
{
    [SerializeField] private List<BuildingUpgradeData> entries = new List<BuildingUpgradeData>();

    private Dictionary<int, List<BuildingUpgradeData>> lookup;

    public IReadOnlyList<BuildingUpgradeData> All => entries;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, List<BuildingUpgradeData>>();
        foreach (var e in entries)
        {
            if (!lookup.ContainsKey(e.buildingId))
                lookup[e.buildingId] = new List<BuildingUpgradeData>();
            lookup[e.buildingId].Add(e);
        }
    }

    public BuildingUpgradeData Get(int _buildingId, int _currentLevel)
    {
        if (lookup == null) BuildLookup();
        if (!lookup.TryGetValue(_buildingId, out var list)) return null;
        return list.Find(d => d.currentLevel == _currentLevel);
    }
}
