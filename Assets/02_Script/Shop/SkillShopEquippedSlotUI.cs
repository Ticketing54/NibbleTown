using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillShopEquippedSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image             iconImage;
    [SerializeField] private TextMeshProUGUI   slotNumberText;
    [SerializeField] private GameObject        selectedOverlay;

public int SlotIndex { get; private set; }

    private SkillShopUI owner;

    public void Init(int slotIndex, SkillShopUI _owner)
    {
        SlotIndex          = slotIndex;
        owner              = _owner;
        slotNumberText.text = (slotIndex + 1).ToString();
        SetSelected(false);
    }

    public void SetSkill(SkillData skill)
    {
        bool hasIcon = skill?.icon != null;
        iconImage.gameObject.SetActive(hasIcon);
        if (hasIcon) iconImage.sprite = skill.icon;
    }

    public void SetSelected(bool selected) => selectedOverlay.SetActive(selected);

    public void OnPointerClick(PointerEventData _) => owner.OnEquippedSlotClicked(this);
}
