using System;
using System.Collections.Generic;

public static partial class GameEvents
{
    // ── 인벤토리 ─────────────────────────────────────────────────
    public static event Action<int, int, int>              OnInventoryChanged;       // itemId, delta, newTotal
    public static event Action                             OnInventoryRefreshRequested;
    public static event Action<IReadOnlyDictionary<int, int>> OnInventoryRefreshed;
    public static event Action<int>                        OnInventorySlotsChanged;
    public static event Action<int, int>                   OnInventoryItemDiscarded;

    public static void RaiseInventoryChanged(int _itemId, int _delta, int _newTotal) => OnInventoryChanged?.Invoke(_itemId, _delta, _newTotal);
    public static void RaiseInventoryRefreshRequested()                              => OnInventoryRefreshRequested?.Invoke();
    public static void RaiseInventoryRefreshed(IReadOnlyDictionary<int, int> _items) => OnInventoryRefreshed?.Invoke(_items);
    public static void RaiseInventorySlotsChanged(int _maxSlots)                     => OnInventorySlotsChanged?.Invoke(_maxSlots);
    public static void RaiseInventoryItemDiscarded(int _itemId, int _count)          => OnInventoryItemDiscarded?.Invoke(_itemId, _count);

    // ── 골드 ─────────────────────────────────────────────────────
    public static event Action<int> OnGoldAcquired;
    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnGoldRefreshed;
    public static event Action      OnGoldRefreshRequested;

    public static void RaiseGoldAcquired(int _amount)  => OnGoldAcquired?.Invoke(_amount);
    public static void RaiseGoldChanged(int _total)    => OnGoldChanged?.Invoke(_total);
    public static void RaiseGoldRefreshed(int _total)  => OnGoldRefreshed?.Invoke(_total);
    public static void RaiseGoldRefreshRequested()     => OnGoldRefreshRequested?.Invoke();
}
