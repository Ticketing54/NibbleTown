using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image      iconImage;
    [SerializeField] private GameObject selectedOverlay;

    public int SkillId { get; private set; }

    private SkillShopUI owner;

    public void Init(SkillData skill, SkillShopUI _owner)
    {
        SkillId = skill.skillId;
        owner   = _owner;

        bool hasIcon = skill.icon != null;
        iconImage.gameObject.SetActive(hasIcon);
        if (hasIcon) iconImage.sprite = skill.icon;

        selectedOverlay.SetActive(false);
    }

    public void SetSelected(bool selected) => selectedOverlay.SetActive(selected);

    public void OnPointerEnter(PointerEventData eventData) => GameEvents.RaiseSkillTooltipShow(SkillId, eventData.position);
    public void OnPointerExit(PointerEventData eventData)  => GameEvents.RaiseSkillTooltipHide();
    public void OnPointerClick(PointerEventData _)         => owner.OnSkillSlotClicked(this);
}
