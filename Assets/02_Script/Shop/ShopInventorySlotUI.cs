using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopInventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public int  ItemId  { get; private set; } = -1;
    public int  Count   { get; private set; }
    public bool IsEmpty => ItemId < 0;

    private ShopUI owner;

    public void Init(ShopUI _owner)
    {
        owner = _owner;
        Clear();
    }

    public void SetItem(int _itemId, Sprite _icon, int _count)
    {
        ItemId            = _itemId;
        Count             = _count;
        iconImage.sprite  = _icon;
        iconImage.enabled = _icon != null;
        countText.text    = _count.ToString();
    }

    public void SetCount(int _count)
    {
        Count          = _count;
        countText.text = _count.ToString();
    }

    public void Clear()
    {
        ItemId            = -1;
        Count             = 0;
        iconImage.sprite  = null;
        iconImage.enabled = false;
        countText.text    = string.Empty;
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        if (IsEmpty) return;
        Debug.Log($"[ShopInvSlotUI] Hover itemId={ItemId}");
        owner.ShowSellTooltip(ItemId, _eventData.position);
    }
    public void OnPointerExit(PointerEventData _eventData)  => owner.HideTooltip();
    public void OnPointerClick(PointerEventData _eventData) { if (!IsEmpty) owner.OnSellSlotClicked(ItemId); }
}
