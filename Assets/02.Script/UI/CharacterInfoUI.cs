using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI apText;
    [SerializeField] private TextMeshProUGUI   levelText;
    [SerializeField] private TextMeshProUGUI   dayText;
    [SerializeField] private Image  apFill;
    [SerializeField] private GameObject endDayBtn;

    private void OnEnable()
    {
        GameEvents.OnAPChanged            += Refresh;
        GameEvents.OnLevelUp              += OnLevelUp;
        GameEvents.OnAPNotEnough           += ShowEndDayButton;
        GameEvents.OnDayChanged           += OnDayChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnAPChanged            -= Refresh;
        GameEvents.OnLevelUp              -= OnLevelUp;
        GameEvents.OnAPNotEnough           -= ShowEndDayButton;
        GameEvents.OnDayChanged           -= OnDayChanged;
    }

    private void Refresh(int _current, int _max)
    {
        apText.text       = _current + " / " + _max;
        apFill.fillAmount = _max > 0 ? (float)_current / _max : 0f;

        if (ActionPointSystem.Instance != null)
            levelText.text = "Lv." + ActionPointSystem.Instance.Level;
    }

    private void OnLevelUp(int _level)
    {
        levelText.text = "Lv." + _level;
        Debug.Log($"[Level Up!] Lv.{_level} — 최대 행동력: {ActionPointSystem.Instance.Max}");
    }

    private void ShowEndDayButton()
    {
        endDayBtn.SetActive(true);
    }

    private void OnDayChanged(int _day)
    {
        dayText.text = "Day " + _day;
        endDayBtn.SetActive(false);
    }
}
