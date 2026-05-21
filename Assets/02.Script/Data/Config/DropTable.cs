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
        foreach (var e in entries)
        {
            if (UnityEngine.Random.value <= e.chance)
                _inventory.AddItem(e.itemId, UnityEngine.Random.Range(e.minCount, e.maxCount + 1));
        }
    }
}
