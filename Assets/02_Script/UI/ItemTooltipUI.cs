using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform itemTooltipBundle;
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Show(int _itemId, Vector2 _screenPos)
    {
        var data = GameDataManager.GetItem(_itemId);
        if (data == null) return;

        if (iconImage)        { iconImage.sprite = data.icon; iconImage.enabled = data.icon != null; }
        if (nameText)         nameText.text        = data.itemName;
        if (typeText)         typeText.text         = data.itemType;
        if (gradeText)        gradeText.text        = data.grade.ToString();
        if (descriptionText)  descriptionText.text  = data.description;

        gameObject.SetActive(true);
        MoveToScreenPos(_screenPos);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void MoveToScreenPos(Vector2 _screenPos)
    {
        Vector2 offset = new Vector2(itemTooltipBundle.rect.width * 0.5f, itemTooltipBundle.rect.height * -0.5f);
        itemTooltipBundle.position = (Vector3)(_screenPos + offset);
    }
}
