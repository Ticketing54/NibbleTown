using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public int ItemId { get; private set; }

    public void Init(int itemId, Sprite icon, int count)
    {
        ItemId            = itemId;
        iconImage.sprite  = icon;
        iconImage.enabled = icon != null;
        countText.text    = count.ToString();
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }
}
