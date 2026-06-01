using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageMonsterEntry
{
    public int monsterIndex;
    public int count;
}

[Serializable]
public class StageData
{
    public List<StageMonsterEntry> monsters;
    public List<StageMonsterEntry> bossMonsters;

    public bool HasBoss => bossMonsters != null && bossMonsters.Count > 0;
}

[CreateAssetMenu(fileName = "StageDatabase", menuName = "NibbleTown/Database/Stage Database")]
public class StageDatabase : ScriptableObject
{
    [SerializeField] private List<StageData> entries = new List<StageData>();

    public StageData Get(int _day)
    {
        if (entries == null || entries.Count == 0) return null;
        int index = Mathf.Clamp(_day - 1, 0, entries.Count - 1);
        return entries[index];
    }
}
