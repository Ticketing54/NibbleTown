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

    public StageData Get(int _day) =>
        _day >= 1 && _day <= entries.Count ? entries[_day - 1] : null;
}
