using System;
using UnityEngine;

public static partial class GameEvents
{
    // ── UI 공용 ───────────────────────────────────────────────────
    public static event Action<bool>   OnProgressBarShow;
    public static event Action<float>  OnProgressBarSetProgress;
    public static event Action<bool, string> OnInteractionTextShow;

    public static void RaiseProgressBarShow(bool _visible)         => OnProgressBarShow?.Invoke(_visible);
    public static void RaiseProgressBarSetProgress(float _value)   => OnProgressBarSetProgress?.Invoke(_value);
    public static void RaiseInteractionTextShow(bool _visible, string _text) => OnInteractionTextShow?.Invoke(_visible, _text);

    // ── 화면 페이드 ───────────────────────────────────────────────
    public static event Action<float, Action> OnFadeInRequested;
    public static event Action<float, Action> OnFadeOutRequested;

    public static void RaiseFadeIn(float duration = -1f, Action onComplete = null)
    {
        if (OnFadeInRequested == null) onComplete?.Invoke();
        else OnFadeInRequested.Invoke(duration, onComplete);
    }

    public static void RaiseFadeOut(float duration = -1f, Action onComplete = null)
    {
        if (OnFadeOutRequested == null) onComplete?.Invoke();
        else OnFadeOutRequested.Invoke(duration, onComplete);
    }

    // ── 툴팁 ─────────────────────────────────────────────────────
    public static event Action<int, Vector2, string> OnItemTooltipShow;
    public static event Action                       OnItemTooltipHide;
    public static event Action<int, Vector2>         OnSkillTooltipShow;
    public static event Action                       OnSkillTooltipHide;

    public static void RaiseItemTooltipShow(int _itemId, Vector2 _screenPos, string _price = null) => OnItemTooltipShow?.Invoke(_itemId, _screenPos, _price);
    public static void RaiseItemTooltipHide()                                                      => OnItemTooltipHide?.Invoke();
    public static void RaiseSkillTooltipShow(int _skillId, Vector2 _screenPos)                     => OnSkillTooltipShow?.Invoke(_skillId, _screenPos);
    public static void RaiseSkillTooltipHide()                                                     => OnSkillTooltipHide?.Invoke();
}
