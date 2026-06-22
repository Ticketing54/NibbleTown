using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemGrade { Common, Uncommon, Rare, Epic, Legendary }

[Serializable]
public class ItemData
{
    public int       itemId;
    public string    itemName;
    public string    itemType;
    public ItemGrade grade;
    public string description;
    public int    maxStack;
    public int    buyPrice;
    public int    sellPrice;
    public bool   isDestroyable = true;
    public Sprite icon;
    public int    skillId = -1;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "NibbleTown/Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> entries = new List<ItemData>();

    private Dictionary<int, ItemData> lookup;

    public IReadOnlyList<ItemData> All => entries;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, ItemData>(entries.Count);
        foreach (var e in entries)
            lookup[e.itemId] = e;
    }

    public ItemData Get(int _itemId)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(_itemId, out var data) ? data : null;
    }
}
