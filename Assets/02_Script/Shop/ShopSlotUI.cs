using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image iconImage;

    public int ItemId { get; private set; }

    private ShopUI owner;

    public void Init(int _itemId, ShopUI _owner)
    {
        ItemId = _itemId;
        owner  = _owner;

        var data = GameDataManager.GetItem(_itemId);
        if (data == null) return;

        iconImage.sprite  = data.icon;
        iconImage.enabled = data.icon != null;
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        Debug.Log($"[ShopSlotUI] Hover itemId={ItemId}");
        owner.ShowBuyTooltip(ItemId, _eventData.position);
    }
    public void OnPointerExit(PointerEventData _eventData)  => owner.HideTooltip();
    public void OnPointerClick(PointerEventData _eventData) => owner.OnBuySlotClicked(ItemId);
}
