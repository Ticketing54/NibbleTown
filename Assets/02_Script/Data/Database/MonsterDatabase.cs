using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterData
{
    public int        monsterIndex;
    public string     monsterName;
    public GameObject prefab;

    [Header("Stats")]
    public int   maxHP          = 50;
    public int   attackDamage   = 10;
    public float attackRange    = 2.5f;
    public float attackInterval = 1.5f;
    public int   dropGold       = 10;
}

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "NibbleTown/Database/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private List<MonsterData> entries = new();

    private Dictionary<int, MonsterData> lookup;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, MonsterData>(entries.Count);
        foreach (MonsterData e in entries)
            lookup[e.monsterIndex] = e;
    }

    public MonsterData Get(int _monsterIndex)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(_monsterIndex, out MonsterData data) ? data : null;
    }
}
