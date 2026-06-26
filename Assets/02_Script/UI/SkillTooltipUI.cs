using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform   tooltipBundle;
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI mpCostText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI priceText;

    private void OnEnable()
    {
        GameEvents.OnSkillTooltipShow += Show;
        GameEvents.OnSkillTooltipHide += Hide;
    }

    private void OnDisable()
    {
        GameEvents.OnSkillTooltipShow -= Show;
        GameEvents.OnSkillTooltipHide -= Hide;
        tooltipBundle.gameObject.SetActive(false);
    }

    private void Show(int skillId, Vector2 screenPos)
    {
        var data = GameDataManager.Skills.Get(skillId);
        if (data == null) return;

        if (iconImage)       { iconImage.sprite = data.icon; iconImage.enabled = data.icon != null; }
        if (nameText)        nameText.text        = data.skillName;
        if (descriptionText) descriptionText.text = data.description;
        if (mpCostText)      mpCostText.text       = $"MP {data.mpCost}";
        if (cooldownText)    cooldownText.text     = $"쿨타임 {data.cooldown}s";
        if (priceText)       priceText.text        = $"{data.price}G";

        tooltipBundle.gameObject.SetActive(true);
        MoveToScreenPos(screenPos);
    }

    private void Hide() => tooltipBundle.gameObject.SetActive(false);

    private void MoveToScreenPos(Vector2 screenPos)
    {
        Vector2 offset = new Vector2(tooltipBundle.rect.width * 0.5f, tooltipBundle.rect.height * -0.5f);
        tooltipBundle.position = (Vector3)(screenPos + offset);
    }
}
