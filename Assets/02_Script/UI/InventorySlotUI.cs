using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public int    ItemId    { get; private set; } = -1;
    public int    Count     { get; private set; }
    public Sprite Icon      => iconImage.sprite;
    public bool   IsEmpty   => ItemId < 0;
    public int    SlotIndex { get; private set; }

    private InventoryUI owner;

    public void Init(InventoryUI _owner, int _index)
    {
        owner     = _owner;
        SlotIndex = _index;
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

    // ── 드래그 & 드랍 ──────────────────────────────────────────

    public void OnPointerEnter(PointerEventData _eventData)
    {
        if (IsEmpty) return;
        owner.ShowTooltip(ItemId, _eventData.position);
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        owner.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData _eventData)
    {
        if (IsEmpty) return;
        owner.BeginDrag(this, _eventData);
    }

    public void OnDrag(PointerEventData _eventData)
    {
        owner.UpdateDrag(_eventData);
    }

    public void OnEndDrag(PointerEventData _eventData)
    {
        owner.EndDrag(this, _eventData);
    }

    public void OnDrop(PointerEventData _eventData)
    {
        owner.DropOnSlot(this);
    }
}
