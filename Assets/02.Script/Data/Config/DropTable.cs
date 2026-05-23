using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDropTable", menuName = "NibbleTown/Drop Table")]
public class DropTable : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public int            itemId;
        public int            minCount;
        public int            maxCount;
        [Range(0f, 1f)]
        public float          chance;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();

    public void GiveTo(Inventory _inventory)
    {
        if (_inventory == null || entries.Count == 0) return;

        float total = 0f;
        foreach (var e in entries) total += e.chance;

        float roll = UnityEngine.Random.value * total;

        foreach (var e in entries)
        {
            roll -= e.chance;
            if (roll <= 0f)
            {
                _inventory.AddItem(e.itemId, UnityEngine.Random.Range(e.minCount, e.maxCount + 1));
                return;
            }
        }

        // 부동소수점 오차 보정 — 마지막 항목 드랍
        var last = entries[entries.Count - 1];
        _inventory.AddItem(last.itemId, UnityEngine.Random.Range(last.minCount, last.maxCount + 1));
    }
}
