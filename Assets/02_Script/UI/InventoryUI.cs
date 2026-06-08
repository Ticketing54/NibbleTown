using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform       slotContainer;
    [SerializeField] private InventorySlotUI slotPrefab;

    private ObjectPool<InventorySlotUI>                     pool;
    private readonly SortedDictionary<int, InventorySlotUI> activeSlots = new SortedDictionary<int, InventorySlotUI>();

    private void Awake()
    {
        pool = new ObjectPool<InventorySlotUI>(
            createFunc:      () => Instantiate(slotPrefab, slotContainer),
            actionOnGet:     slot => slot.gameObject.SetActive(true),
            actionOnRelease: slot => slot.gameObject.SetActive(false),
            actionOnDestroy: slot => Destroy(slot.gameObject)
        );
    }

    public void Open()
    {
        Populate();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnInventoryChanged += OnInventoryChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryChanged -= OnInventoryChanged;
    }

    private void OnDestroy()
    {
        pool.Dispose();
    }

    private void Populate()
    {
        ClearSlots();

        GameEvents.OnInventoryRefreshed += OnInventoryRefreshed;
        GameEvents.RaiseInventoryRefreshRequested();
        GameEvents.OnInventoryRefreshed -= OnInventoryRefreshed;
    }

    private void OnInventoryRefreshed(IReadOnlyDictionary<int, int> items)
    {
        foreach (var kv in items)
            CreateSlot(kv.Key, kv.Value);
    }

    private void OnInventoryChanged(int itemId, int count)
    {
        if (count <= 0)
            ReleaseSlot(itemId);
        else
            CreateOrUpdate(itemId, count);
    }

    private void CreateOrUpdate(int itemId, int count)
    {
        if (activeSlots.TryGetValue(itemId, out var slot))
        {
            slot.SetCount(count);
            return;
        }

        CreateSlot(itemId, count);
    }

    private void CreateSlot(int itemId, int count)
    {
        var slot = pool.Get();
        slot.transform.SetParent(slotContainer, false);

        var data = GameDataManager.GetItem(itemId);
        slot.Init(itemId, data?.icon, count);
        activeSlots[itemId] = slot;

        RefreshSiblingOrder();
    }

    private void ReleaseSlot(int itemId)
    {
        if (!activeSlots.TryGetValue(itemId, out var slot)) return;
        activeSlots.Remove(itemId);
        pool.Release(slot);
        RefreshSiblingOrder();
    }

    private void ClearSlots()
    {
        foreach (var kv in activeSlots)
            pool.Release(kv.Value);
        activeSlots.Clear();
    }

    private void RefreshSiblingOrder()
    {
        int index = 0;
        foreach (var kv in activeSlots)
            kv.Value.transform.SetSiblingIndex(index++);
    }
}
