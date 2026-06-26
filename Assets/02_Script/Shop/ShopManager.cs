using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour, IShopManager, INpcConversationHandler
{
    public IReadOnlyList<int> CurrentEntries => currentItems;
    public IReadOnlyList<int> CurrentItems   => currentItems;

    private readonly List<int> currentItems = new();
    private int currentDay = 1;

    private void OnEnable()
    {
        GameEvents.OnDayChanged  += OnDayChanged;
        GameEvents.OnDayBegin    += Refresh;
        GameEvents.OnDayAdvanced += Refresh;
    }

    private void OnDisable()
    {
        GameEvents.OnDayChanged  -= OnDayChanged;
        GameEvents.OnDayBegin    -= Refresh;
        GameEvents.OnDayAdvanced -= Refresh;
    }

    public void OnConversationStarted() => GameEvents.RaiseShopOpen(currentItems);
    public void OnConversationEnded()   => GameEvents.RaiseShopClose();

    private void OnDayChanged(int _day) => currentDay = _day;

    private void Refresh()
    {
        currentItems.Clear();

        var shop = GameDataManager.Shop;
        if (shop == null || shop.tiers.Count == 0) return;

        var pool  = BuildPool(shop);
        int count = Mathf.Min(shop.slotCount, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int pick = Random.Range(0, pool.Count);
            currentItems.Add(pool[pick].itemId);
            pool.RemoveAt(pick);
        }
    }

    private List<ShopData.Entry> BuildPool(ShopData _shop)
    {
        var pool = new List<ShopData.Entry>();
        foreach (var tier in _shop.tiers)
            if (tier.unlockAtDay <= currentDay)
                pool.AddRange(tier.items);
        return pool;
    }
}
