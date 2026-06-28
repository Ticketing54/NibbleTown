using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image             iconImage;
    [SerializeField] private Image             cooldownOverlay;
    [SerializeField] private TextMeshProUGUI   cooldownText;

public void SetSkill(SkillData skill)
    {
        bool hasIcon = skill?.icon != null;
        iconImage.gameObject.SetActive(hasIcon);
        if (hasIcon) iconImage.sprite = skill.icon;

        SetCooldown(0f, 0f);
    }

    public void UpdateCooldown(float ratio, float remaining)
    {
        bool onCooldown = ratio > 0f;

        cooldownOverlay.fillAmount = ratio;
        cooldownOverlay.gameObject.SetActive(onCooldown);

        if (onCooldown)
            cooldownText.text = Mathf.CeilToInt(remaining).ToString();

        cooldownText.gameObject.SetActive(onCooldown);
    }

    private void SetCooldown(float ratio, float remaining)
    {
        cooldownOverlay.fillAmount = ratio;
        cooldownOverlay.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
    }
}
