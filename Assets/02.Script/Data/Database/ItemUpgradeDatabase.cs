using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemUpgradeData
{
    public int    targetItemId;
    public string targetItemName;
    public string currentGrade;
    public int    resultItemId;
    public string resultItemName;
    public string nextGrade;
    public int    requiredItem1Id;
    public int    requiredItem1Count;
    public int    requiredItem2Id;
    public int    requiredItem2Count;
    public int    requiredItem3Id;
    public int    requiredItem3Count;
    public int    requiredGold;
    public float  successRate;
}

[CreateAssetMenu(fileName = "ItemUpgradeDatabase", menuName = "NibbleTown/Database/Item Upgrade Database")]
public class ItemUpgradeDatabase : ScriptableObject
{
    [SerializeField] private List<ItemUpgradeData> entries = new List<ItemUpgradeData>();

    private Dictionary<int, ItemUpgradeData> lookup;

    public IReadOnlyList<ItemUpgradeData> All => entries;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, ItemUpgradeData>(entries.Count);
        foreach (var e in entries)
            lookup[e.targetItemId] = e;
    }

    public ItemUpgradeData Get(int _targetItemId)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(_targetItemId, out var data) ? data : null;
    }
}
