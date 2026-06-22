using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform   itemTooltipBundle;
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;

    private void OnEnable()
    {
        GameEvents.OnItemTooltipShow += Show;
        GameEvents.OnItemTooltipHide += Hide;
    }

    private void OnDisable()
    {
        GameEvents.OnItemTooltipShow -= Show;
        GameEvents.OnItemTooltipHide -= Hide;
        itemTooltipBundle.gameObject.SetActive(false);
    }

    private void Show(int _itemId, Vector2 _screenPos, string _price)
    {
        var data = GameDataManager.GetItem(_itemId);
        Debug.Log($"[ItemTooltipUI] Show itemId={_itemId} data={(data == null ? "null" : data.itemName)} price={_price}");
        if (data == null) return;

        if (iconImage)       { iconImage.sprite = data.icon; iconImage.enabled = data.icon != null; }
        if (nameText)        nameText.text       = data.itemName;
        if (typeText)        typeText.text        = data.itemType;
        if (gradeText)       gradeText.text       = data.grade.ToString();
        if (descriptionText) descriptionText.text = data.description;
        if (priceText)       { priceText.text = _price ?? string.Empty; priceText.gameObject.SetActive(_price != null); }

        itemTooltipBundle.gameObject.SetActive(true);
        MoveToScreenPos(_screenPos);
    }

    private void Hide()
    {
        itemTooltipBundle.gameObject.SetActive(false);
    }

    private void MoveToScreenPos(Vector2 _screenPos)
    {
        Vector2 offset = new Vector2(itemTooltipBundle.rect.width * 0.5f, itemTooltipBundle.rect.height * -0.5f);
        itemTooltipBundle.position = (Vector3)(_screenPos + offset);
    }
}
