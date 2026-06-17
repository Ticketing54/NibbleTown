using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform       slotContainer;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Image           dragGhost;
    [SerializeField] private ItemTooltipUI   tooltip;

    private const int DefaultSlots = 9;

    private readonly List<InventorySlotUI> slots      = new List<InventorySlotUI>();
    private readonly Dictionary<int, int>  itemToSlot = new Dictionary<int, int>();

    private InventorySlotUI draggingFrom;
    private bool            dropHandled;

    private void Awake()
    {
        dragGhost.raycastTarget = false;
        dragGhost.gameObject.SetActive(false);

        for (int i = 0; i < DefaultSlots; i++)
            AddSlot();

    }

    private void OnEnable()
    {
        GameEvents.OnInventoryChanged      += OnInventoryChanged;
        GameEvents.OnInventorySlotsChanged += OnSlotsChanged;
        RefreshFromInventory();
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryChanged      -= OnInventoryChanged;
        GameEvents.OnInventorySlotsChanged -= OnSlotsChanged;
        tooltip?.Hide();
    }

    public void Open()  => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    // ── 슬롯 수 변경 ──────────────────────────────────────────

    private void OnSlotsChanged(int _maxSlots)
    {
        while (slots.Count < _maxSlots)
            AddSlot();
    }

    private void AddSlot()
    {
        var slot = Instantiate(slotPrefab, slotContainer);
        slot.gameObject.SetActive(true);
        slot.Init(this, slots.Count);
        slots.Add(slot);
    }

    // ── 아이템 변경 ────────────────────────────────────────────

    private void OnInventoryChanged(int _itemId, int _count)
    {
        if (_count <= 0) ClearItem(_itemId);
        else             SetOrUpdateItem(_itemId, _count);
    }

    private void SetOrUpdateItem(int _itemId, int _count)
    {
        if (itemToSlot.TryGetValue(_itemId, out int idx))
        {
            slots[idx].SetCount(_count);
            return;
        }

        int emptyIdx = FindEmptySlot();
        if (emptyIdx < 0) return;

        itemToSlot[_itemId] = emptyIdx;
        var icon = GameDataManager.GetItem(_itemId)?.icon;
        slots[emptyIdx].SetItem(_itemId, icon, _count);
    }

    private void ClearItem(int _itemId)
    {
        if (!itemToSlot.TryGetValue(_itemId, out int idx)) return;
        slots[idx].Clear();
        itemToSlot.Remove(_itemId);
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
            if (slots[i].IsEmpty) return i;
        return -1;
    }

    // ── 전체 새로고침 ──────────────────────────────────────────

    private void RefreshFromInventory()
    {
        foreach (var slot in slots) slot.Clear();
        itemToSlot.Clear();

        GameEvents.OnInventoryRefreshed += OnInventoryRefreshed;
        GameEvents.RaiseInventoryRefreshRequested();
        GameEvents.OnInventoryRefreshed -= OnInventoryRefreshed;
    }

    private void OnInventoryRefreshed(IReadOnlyDictionary<int, int> _items)
    {
        foreach (var kv in _items)
            SetOrUpdateItem(kv.Key, kv.Value);
    }

    // ── 툴팁 ──────────────────────────────────────────────────

    public void ShowTooltip(int _itemId, Vector2 _screenPos) => tooltip?.Show(_itemId, _screenPos);
    public void HideTooltip()                                 => tooltip?.Hide();

    // ── 드래그 & 드랍 ──────────────────────────────────────────

    public void BeginDrag(InventorySlotUI _slot, PointerEventData _eventData)
    {
        draggingFrom         = _slot;
        dropHandled          = false;
        dragGhost.sprite     = _slot.Icon;
        dragGhost.gameObject.SetActive(true);
        MoveGhost(_eventData.position);
        tooltip?.Hide();
    }

    public void UpdateDrag(PointerEventData _eventData)
    {
        if (draggingFrom == null) return;
        MoveGhost(_eventData.position);
    }

    public void EndDrag(InventorySlotUI _slot, PointerEventData _eventData)
    {
        dragGhost.gameObject.SetActive(false);

        if (!dropHandled && draggingFrom != null)
        {
            var panelRect = transform as RectTransform;
            bool outsidePanel = !RectTransformUtility.RectangleContainsScreenPoint(
                panelRect, _eventData.position, _eventData.pressEventCamera);

            if (outsidePanel)
            {
                var data = GameDataManager.GetItem(draggingFrom.ItemId);
                if (data == null || data.isDestroyable)
                    GameEvents.RaiseInventoryItemDiscarded(draggingFrom.ItemId, draggingFrom.Count);
            }
        }

        draggingFrom = null;
    }

    public void DropOnSlot(InventorySlotUI _target)
    {
        if (draggingFrom == null) return;
        dropHandled = true;
        if (draggingFrom == _target) return;
        SwapSlots(draggingFrom, _target);
    }

    private void SwapSlots(InventorySlotUI _a, InventorySlotUI _b)
    {
        if (!_a.IsEmpty) itemToSlot[_a.ItemId] = _b.SlotIndex;
        if (!_b.IsEmpty) itemToSlot[_b.ItemId] = _a.SlotIndex;

        int    aId    = _a.ItemId;
        Sprite aIcon  = _a.Icon;
        int    aCount = _a.Count;

        if (_b.IsEmpty) _a.Clear();
        else            _a.SetItem(_b.ItemId, _b.Icon, _b.Count);

        if (aId < 0) _b.Clear();
        else         _b.SetItem(aId, aIcon, aCount);
    }

    private void MoveGhost(Vector2 _screenPos)
    {
        dragGhost.transform.position = (Vector3)_screenPos;
    }
}
