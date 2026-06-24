using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image             iconImage;
    [SerializeField] private Image             cooldownOverlay;
    [SerializeField] private TextMeshProUGUI   cooldownText;

    private static readonly Color EmptyColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    public void SetSkill(SkillData skill)
    {
        if (skill != null)
        {
            iconImage.sprite = skill.icon;
            iconImage.color  = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color  = EmptyColor;
        }

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
