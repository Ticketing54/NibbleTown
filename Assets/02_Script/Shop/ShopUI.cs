using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private const int InventorySlotCount = 9;

    [Header("상점")]
    [SerializeField] private Transform  shopSlotContainer;
    [SerializeField] private ShopSlotUI shopSlotPrefab;

    [Header("인벤토리")]
    [SerializeField] private Transform           inventoryContainer;
    [SerializeField] private ShopInventorySlotUI inventorySlotPrefab;

    [Header("확인창")]
    [SerializeField] private GameObject      confirmPanel;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button          confirmBtn;
    [SerializeField] private Button          cancelBtn;

    [Header("공통")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button          closeButton;
    [SerializeField] private GameObject      panel;

    private readonly List<ShopSlotUI>             shopSlots  = new();
    private readonly List<ShopInventorySlotUI>     invSlots   = new();
    private readonly Dictionary<int, int>          itemToSlot = new();

    private int  pendingItemId;
    private bool pendingIsBuy;

    private void Awake()
    {
        Debug.Log($"[ShopUI] Awake — confirmPanel={confirmPanel != null} confirmBtn={confirmBtn != null} cancelBtn={cancelBtn != null}");

        confirmPanel.SetActive(false);

        confirmBtn.onClick.AddListener(OnConfirm);
        Debug.Log("[ShopUI] confirmBtn 리스너 등록 완료");
        cancelBtn.onClick.AddListener(OnCancelConfirm);
        closeButton?.onClick.AddListener(GameEvents.RaiseNpcConversationCloseRequested);

        for (int i = 0; i < InventorySlotCount; i++)
        {
            var slot = Instantiate(inventorySlotPrefab, inventoryContainer, false);
            slot.gameObject.SetActive(true);
            slot.Init(this);
            invSlots.Add(slot);
        }

        panel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnShopOpen              += Open;
        GameEvents.OnShopClose             += Close;
        GameEvents.OnInventoryChanged      += OnInventoryChanged;
        GameEvents.OnInventorySlotsChanged += OnSlotsChanged;
        GameEvents.OnGoldChanged           += OnGoldChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnShopOpen              -= Open;
        GameEvents.OnShopClose             -= Close;
        GameEvents.OnInventoryChanged      -= OnInventoryChanged;
        GameEvents.OnInventorySlotsChanged -= OnSlotsChanged;
        GameEvents.OnGoldChanged           -= OnGoldChanged;
        GameEvents.RaiseItemTooltipHide();
    }

    // ── 열기 / 닫기 ───────────────────────────────────────────

    public void Open(IReadOnlyList<int> items)
    {
        panel.SetActive(true);
        confirmPanel.SetActive(false);
        PopulateShop(items);
        RefreshInventory();
        GameEvents.OnGoldRefreshed += OnGoldChanged;
        GameEvents.RaiseGoldRefreshRequested();
        GameEvents.OnGoldRefreshed -= OnGoldChanged;
    }

    public void Close() => panel.SetActive(false);

    // ── 상점 슬롯 구성 ────────────────────────────────────────

    private void PopulateShop(IReadOnlyList<int> items)
    {
        foreach (var s in shopSlots) Destroy(s.gameObject);
        shopSlots.Clear();

        if (items == null || items.Count == 0) return;

        foreach (int itemId in items)
        {
            var slot = Instantiate(shopSlotPrefab, shopSlotContainer, false);
            slot.gameObject.SetActive(true);
            slot.Init(itemId, this);
            shopSlots.Add(slot);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(shopSlotContainer as RectTransform);
    }

    // ── 인벤토리 표시 ─────────────────────────────────────────

    private void RefreshInventory()
    {
        foreach (var s in invSlots) s.Clear();
        itemToSlot.Clear();

        GameEvents.OnInventoryRefreshed += OnInventoryRefreshed;
        GameEvents.RaiseInventoryRefreshRequested();
        GameEvents.OnInventoryRefreshed -= OnInventoryRefreshed;
    }

    private void OnInventoryRefreshed(IReadOnlyDictionary<int, int> _items)
    {
        foreach (var kv in _items) SetOrUpdateInvSlot(kv.Key, kv.Value);
    }

    private void OnInventoryChanged(int _itemId, int _delta, int _newTotal)
    {
        if (!panel.activeSelf) return;
        if (_newTotal <= 0) ClearInvSlot(_itemId);
        else                SetOrUpdateInvSlot(_itemId, _newTotal);
    }

    private void SetOrUpdateInvSlot(int _itemId, int _count)
    {
        if (itemToSlot.TryGetValue(_itemId, out int idx))
        {
            invSlots[idx].SetCount(_count);
            return;
        }

        int empty = FindEmptyInvSlot();
        if (empty < 0) return;

        itemToSlot[_itemId] = empty;
        var icon = GameDataManager.GetItem(_itemId)?.icon;
        invSlots[empty].SetItem(_itemId, icon, _count);
    }

    private void ClearInvSlot(int _itemId)
    {
        if (!itemToSlot.TryGetValue(_itemId, out int idx)) return;
        invSlots[idx].Clear();
        itemToSlot.Remove(_itemId);
    }

    private int FindEmptyInvSlot()
    {
        for (int i = 0; i < invSlots.Count; i++)
            if (invSlots[i].IsEmpty) return i;
        return -1;
    }

    // ── 슬롯 수 동기화 ────────────────────────────────────────

    private void OnSlotsChanged(int _maxSlots)
    {
        while (invSlots.Count < _maxSlots)
        {
            var slot = Instantiate(inventorySlotPrefab, inventoryContainer, false);
            slot.gameObject.SetActive(true);
            slot.Init(this);
            invSlots.Add(slot);
        }
        Debug.Log($"[ShopUI] 인벤토리 슬롯 수 동기화: {invSlots.Count}");
    }

    // ── 골드 ──────────────────────────────────────────────────

    private void OnGoldChanged(int _total)
    {
        if (goldText != null) goldText.text = $"{_total}G";
    }


    // ── 확인창 ────────────────────────────────────────────────

    public void OnBuySlotClicked(int _itemId)
    {
        Debug.Log($"[ShopUI] OnBuySlotClicked itemId={_itemId}");
        var data = GameDataManager.GetItem(_itemId);
        if (data == null) return;

        pendingItemId = _itemId;
        pendingIsBuy  = true;
        confirmText.text = $"{data.itemName}을(를)\n{data.buyPrice}G에 구매하시겠습니까?";
        confirmPanel.SetActive(true);
    }

    public void OnSellSlotClicked(int _itemId)
    {
        var data = GameDataManager.GetItem(_itemId);
        if (data == null) return;

        pendingItemId = _itemId;
        pendingIsBuy  = false;
        confirmText.text = $"{data.itemName}을(를)\n{data.sellPrice}G에 판매하시겠습니까?";
        confirmPanel.SetActive(true);
    }

    private void OnConfirm()
    {
        confirmPanel.SetActive(false);

        var inv  = FindFirstObjectByType<Inventory>();
        var data = GameDataManager.GetItem(pendingItemId);

        Debug.Log($"[ShopUI] OnConfirm — isBuy={pendingIsBuy} itemId={pendingItemId} inv={(inv == null ? "null" : "ok")} data={(data == null ? "null" : data.itemName)} gold={inv?.Gold} buyPrice={data?.buyPrice}");

        if (inv == null || data == null) return;

        if (pendingIsBuy)
        {
            if (!inv.SpendGold(data.buyPrice))
            {
                Debug.LogWarning($"[ShopUI] 골드 부족 — 보유 {inv.Gold}G / 필요 {data.buyPrice}G");
                return;
            }
            inv.AddItem(data.itemId, 1);
        }
        else
        {
            if (!inv.TryRemove(data.itemId, 1)) return;
            inv.AddGold(data.sellPrice);
            GameEvents.RaiseGoldAcquired(data.sellPrice);
        }
    }

    private void OnCancelConfirm() => confirmPanel.SetActive(false);

    // ── 툴팁 ──────────────────────────────────────────────────

    public void ShowBuyTooltip(int _itemId, Vector2 _screenPos)
    {
        var data = GameDataManager.GetItem(_itemId);
        GameEvents.RaiseItemTooltipShow(_itemId, _screenPos, data != null ? $"구매 {data.buyPrice}G" : null);
    }

    public void ShowSellTooltip(int _itemId, Vector2 _screenPos)
    {
        var data = GameDataManager.GetItem(_itemId);
        GameEvents.RaiseItemTooltipShow(_itemId, _screenPos, data != null ? $"판매 {data.sellPrice}G" : null);
    }

    public void HideTooltip() => GameEvents.RaiseItemTooltipHide();
}
