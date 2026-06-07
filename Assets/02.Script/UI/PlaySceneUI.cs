using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaySceneUI : MonoBehaviour
{
    [Serializable]
    private class ProgressState
    {
        public TextMeshProUGUI progressText;
        public Image           fillImage;
    }
    
    [SerializeField] private GameObject uiBunddle;
    [SerializeField] private ProgressState hpProgressState;
    [SerializeField] private ProgressState mpProgressState;
    [SerializeField] private ProgressState apProgressState;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private GameObject      endDayBtn;
    [SerializeField] private ProgressState   mainBuildingHPState;

    private void Start()
    {
        uiBunddle?.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnAPChanged        += RefreshAP;
        GameEvents.OnHPChanged        += RefreshHP;
        GameEvents.OnMPChanged        += RefreshMP;
        GameEvents.OnPhaseChanged     += OnPhaseChanged;
        GameEvents.OnDayBegin              += ShowHUD;
        GameEvents.OnNightBegin            += ShowHUD;
        GameEvents.OnNightRequested        += HideHUD;
        GameEvents.OnNextDayRequested      += HideHUD;
        GameEvents.OnMainBuildingHPChanged += RefreshMainBuildingHP;
    }

    private void OnDisable()
    {
        GameEvents.OnAPChanged             -= RefreshAP;
        GameEvents.OnHPChanged             -= RefreshHP;
        GameEvents.OnMPChanged             -= RefreshMP;
        GameEvents.OnPhaseChanged          -= OnPhaseChanged;
        GameEvents.OnDayBegin              -= ShowHUD;
        GameEvents.OnNightBegin            -= ShowHUD;
        GameEvents.OnNightRequested        -= HideHUD;
        GameEvents.OnNextDayRequested      -= HideHUD;
        GameEvents.OnMainBuildingHPChanged -= RefreshMainBuildingHP;
    }

    private void ShowHUD() => uiBunddle?.SetActive(true);
    private void HideHUD() => uiBunddle?.SetActive(false);

    public void OnEndDayButtonClick()
    {
        GameEvents.RaiseNightRequested();
    }

    private void RefreshHP(int _current, int _max)
    {
        hpProgressState.progressText.text    = _current + " / " + _max;
        hpProgressState.fillImage.fillAmount = _max > 0 ? (float)_current / _max : 0f;
    }

    private void RefreshMP(int _current, int _max)
    {
        mpProgressState.progressText.text    = _current + " / " + _max;
        mpProgressState.fillImage.fillAmount = _max > 0 ? (float)_current / _max : 0f;
    }

    private void RefreshAP(int _current, int _max)
    {
        apProgressState.progressText.text    = _current + " / " + _max;
        apProgressState.fillImage.fillAmount = _max > 0 ? (float)_current / _max : 0f;
    }

    private void OnDayChanged(int _day) { }

    private void OnPhaseChanged(int _day, StagePhase _phase)
    {
        dayText.text = _phase == StagePhase.Day ? $"DAY {_day}" : $"NIGHT {_day}";
        endDayBtn.SetActive(_phase == StagePhase.Day);
    }

    private void RefreshMainBuildingHP(int _current, int _max)
    {
        if (mainBuildingHPState == null) return;
        mainBuildingHPState.progressText.text    = _current + " / " + _max;
        mainBuildingHPState.fillImage.fillAmount = _max > 0 ? (float)_current / _max : 0f;
    }
}
