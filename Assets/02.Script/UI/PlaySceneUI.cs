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

    [SerializeField] private ProgressState hpProgressState;
    [SerializeField] private ProgressState mpProgressState;
    [SerializeField] private ProgressState apProgressState;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private GameObject      endDayBtn;

    private void OnEnable()
    {
        GameEvents.OnAPChanged      += RefreshAP;
        GameEvents.OnHPChanged      += RefreshHP;
        GameEvents.OnMPChanged      += RefreshMP;
        GameEvents.OnDayChanged     += OnDayChanged;
        GameEvents.OnPhaseChanged   += OnPhaseChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnAPChanged      -= RefreshAP;
        GameEvents.OnHPChanged      -= RefreshHP;
        GameEvents.OnMPChanged      -= RefreshMP;
        GameEvents.OnDayChanged     -= OnDayChanged;
        GameEvents.OnPhaseChanged   -= OnPhaseChanged;
    }

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

    private void OnDayChanged(int _day)
    {
        dayText.text = "Day " + _day;
    }

    private void OnPhaseChanged(int _day, StagePhase _phase)
    {
        endDayBtn.SetActive(_phase == StagePhase.Day);
    }
}
