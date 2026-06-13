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
        if (_inventory == null) return;
        if (!Roll(out int itemId, out int count)) return;
        _inventory.AddItem(itemId, count);
    }

    public bool Roll(out int itemId, out int count)
    {
        itemId = 0; count = 0;
        if (entries.Count == 0) return false;

        float total = 0f;
        foreach (var e in entries) total += e.chance;

        float roll = UnityEngine.Random.value * total;
        foreach (var e in entries)
        {
            roll -= e.chance;
            if (roll <= 0f)
            {
                itemId = e.itemId;
                count  = UnityEngine.Random.Range(e.minCount, e.maxCount + 1);
                return true;
            }
        }

        var last = entries[entries.Count - 1];
        itemId = last.itemId;
        count  = UnityEngine.Random.Range(last.minCount, last.maxCount + 1);
        return true;
    }
}
