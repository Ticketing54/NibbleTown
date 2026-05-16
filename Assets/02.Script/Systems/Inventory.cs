using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly Dictionary<int, int> items = new Dictionary<int, int>();

    // ── 추가 (생산·사냥 등 자동 획득) ────────────────────────

    public void AddItem(int _itemId, int _count)
    {
        if (_count <= 0) return;

        if (!items.ContainsKey(_itemId))
            items[_itemId] = 0;
Debug.Log(_itemId +" /" +_count);
        items[_itemId] += _count;
        GameEvents.RaiseInventoryChanged(_itemId, items[_itemId]);
    }

    // ── 제거 (건설·제작 시 소비) ──────────────────────────────

    public bool TryRemove(int _itemId, int _count)
    {
        if (!Has(_itemId, _count)) return false;

        items[_itemId] -= _count;
        if (items[_itemId] == 0)
            items.Remove(_itemId);

        GameEvents.RaiseInventoryChanged(_itemId, GetCount(_itemId));
        return true;
    }

    // 재료가 모두 충족될 때만 전부 소비 (부족하면 아무것도 소비 안 함)
    public bool TryConsumeAll(IReadOnlyList<ItemSlot> _requirements)
    {
        foreach (var req in _requirements)
            if (!Has(req.itemId, req.count)) return false;

        foreach (var req in _requirements)
            TryRemove(req.itemId, req.count);

        return true;
    }

    // ── 조회 ──────────────────────────────────────────────────

    public bool Has(int _itemId, int _count = 1) =>
        items.TryGetValue(_itemId, out int c) && c >= _count;

    public int GetCount(int _itemId) =>
        items.TryGetValue(_itemId, out int c) ? c : 0;

    // UI 초기 렌더링용 — 현재 전체 목록을 읽기 전용으로 반환
    public IReadOnlyDictionary<int, int> GetAllItems() => items;

    // ── 저장/불러오기 ─────────────────────────────────────────

    public List<ItemSlot> GetSaveData()
    {
        var result = new List<ItemSlot>(items.Count);
        foreach (var kv in items)
            result.Add(new ItemSlot(kv.Key, kv.Value));
        return result;
    }

    public void LoadSaveData(List<ItemSlot> _data)
    {
        items.Clear();
        foreach (var slot in _data)
            items[slot.itemId] = slot.count;
    }
}
