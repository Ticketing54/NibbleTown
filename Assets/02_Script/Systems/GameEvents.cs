using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action<CharacterData>   OnCharacterDataChanged;
    public static event Action<int, int>        OnAPChanged;
    public static event Action<int, int>        OnHPChanged;
    public static event Action<int, int>        OnMPChanged;
    public static event Action                  OnAPNotEnough;
    public static event Action<int>             OnDayChanged;
    public static event Action<int, StagePhase> OnPhaseChanged;
    public static event Action<bool>            OnProgressBarShow;
    public static event Action<float>           OnProgressBarSetProgress;
    public static event Action<bool, string>    OnInteractionTextShow;
    public static event Action<int, int, int>                         OnInventoryChanged; // itemId, delta, newTotal
    public static event Action                                        OnInventoryRefreshRequested;
    public static event Action<IReadOnlyDictionary<int, int>>         OnInventoryRefreshed;
    public static event Action<int>                                   OnInventorySlotsChanged;
    public static event Action<int, int>                              OnInventoryItemDiscarded;

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

    // ── 플레이어 입력 ──────────────────────────────────────────────
    public static event Action<bool> OnPlayerInputLocked;
    public static event Action<bool> OnNpcConversationChanged;
    public static event Action       OnNpcConversationCloseRequested;

    // ── 페이즈 전환 요청 (외부 → PlaySceneController) ─────────────
    public static event Action OnNightRequested;
    public static event Action OnNextDayRequested;

    // ── 페이즈 전환 확정 (PlaySceneController → StageManager 등) ──
    public static event Action OnDayBegin;
    public static event Action OnNightBegin;
    public static event Action OnDayAdvanced; // 밤 → 낮 전환 시에만 (2일차~)

    public static void RaiseCharacterDataChanged(CharacterData _data)            => OnCharacterDataChanged?.Invoke(_data);
    public static void RaiseAPChanged(int _current, int _max)                    => OnAPChanged?.Invoke(_current, _max);
    public static void RaiseHPChanged(int _current, int _max)                    => OnHPChanged?.Invoke(_current, _max);
    public static void RaiseMPChanged(int _current, int _max)                    => OnMPChanged?.Invoke(_current, _max);
    public static void RaiseAPNotEnough()                                        => OnAPNotEnough?.Invoke();
    public static void RaiseDayChanged(int _day)                                 => OnDayChanged?.Invoke(_day);
    public static void RaisePhaseChanged(int _day, StagePhase _phase)            => OnPhaseChanged?.Invoke(_day, _phase);
    public static void RaiseProgressBarShow(bool _visible)                       => OnProgressBarShow?.Invoke(_visible);
    public static void RaiseProgressBarSetProgress(float _value)                 => OnProgressBarSetProgress?.Invoke(_value);
    public static void RaiseInteractionTextShow(bool _visible, string _text)     => OnInteractionTextShow?.Invoke(_visible, _text);
    public static void RaiseInventoryChanged(int _itemId, int _delta, int _newTotal)          => OnInventoryChanged?.Invoke(_itemId, _delta, _newTotal);
    public static void RaiseInventoryRefreshRequested()                                       => OnInventoryRefreshRequested?.Invoke();
    public static void RaiseInventoryRefreshed(IReadOnlyDictionary<int, int> _items)         => OnInventoryRefreshed?.Invoke(_items);
    public static void RaiseInventorySlotsChanged(int _maxSlots)                             => OnInventorySlotsChanged?.Invoke(_maxSlots);
    public static void RaiseInventoryItemDiscarded(int _itemId, int _count)                  => OnInventoryItemDiscarded?.Invoke(_itemId, _count);

    public static void RaisePlayerInputLocked(bool _locked)                       => OnPlayerInputLocked?.Invoke(_locked);
    public static void RaiseNpcConversationChanged(bool _active) => OnNpcConversationChanged?.Invoke(_active);

    // ── 상점 UI 이벤트 ────────────────────────────────────────────
    public static event Action<IReadOnlyList<int>> OnShopOpen;
    public static event Action                     OnShopClose;
    public static event Action<IReadOnlyList<int>> OnSkillShopOpen;
    public static event Action                     OnSkillShopClose;

    public static void RaiseShopOpen(IReadOnlyList<int> _items)    => OnShopOpen?.Invoke(_items);
    public static void RaiseShopClose()                            => OnShopClose?.Invoke();
    public static void RaiseSkillShopOpen(IReadOnlyList<int> _skills) => OnSkillShopOpen?.Invoke(_skills);
    public static void RaiseSkillShopClose()                          => OnSkillShopClose?.Invoke();
    public static void RaiseNpcConversationCloseRequested()                        => OnNpcConversationCloseRequested?.Invoke();

    // ── 아이템 툴팁 ───────────────────────────────────────────────
    public static event Action<int, Vector2, string> OnItemTooltipShow;
    public static event Action                       OnItemTooltipHide;

    public static void RaiseItemTooltipShow(int _itemId, Vector2 _screenPos, string _price = null)
        => OnItemTooltipShow?.Invoke(_itemId, _screenPos, _price);
    public static void RaiseItemTooltipHide() => OnItemTooltipHide?.Invoke();

    // ── 스킬 툴팁 ────────────────────────────────────────────────
    public static event Action<int, Vector2> OnSkillTooltipShow;
    public static event Action               OnSkillTooltipHide;

    public static void RaiseSkillTooltipShow(int _skillId, Vector2 _screenPos) => OnSkillTooltipShow?.Invoke(_skillId, _screenPos);
    public static void RaiseSkillTooltipHide()                                  => OnSkillTooltipHide?.Invoke();

    // ── 획득 알림 ─────────────────────────────────────────────────
    public static event Action<int> OnGoldAcquired;        // amount
    public static event Action<int> OnGoldChanged;         // total
    public static event Action      OnGoldRefreshRequested;

    public static void RaiseGoldAcquired(int _amount)  => OnGoldAcquired?.Invoke(_amount);
    public static void RaiseGoldChanged(int _total)    => OnGoldChanged?.Invoke(_total);
    public static void RaiseGoldRefreshRequested()     => OnGoldRefreshRequested?.Invoke();

    // ── 스킬 ──────────────────────────────────────────────────────
    public static event Action<int, int>          OnSkillEquipped;     // slotIndex, skillId (-1 = 해제)
    public static event Action<int>               OnSkillUsed;         // slotIndex
    public static event Action<int, float, float> OnSkillCooldownTick; // slotIndex, ratio, remaining

    public static void RaiseSkillEquipped(int _slotIndex, int _skillId)                          => OnSkillEquipped?.Invoke(_slotIndex, _skillId);
    public static void RaiseSkillUsed(int _slotIndex)                                            => OnSkillUsed?.Invoke(_slotIndex);
    public static void RaiseSkillCooldownTick(int _slotIndex, float _ratio, float _remaining)    => OnSkillCooldownTick?.Invoke(_slotIndex, _ratio, _remaining);

    public static event Action<int>          OnMonsterDied;
    public static event Action<BuildingHealth> OnBuildingDestroyed;
    public static event Action<int, int>     OnMainBuildingHPChanged;

    public static void RaiseNightRequested()                        => OnNightRequested?.Invoke();
    public static void RaiseNextDayRequested()                      => OnNextDayRequested?.Invoke();
    public static void RaiseDayBegin()                              => OnDayBegin?.Invoke();
    public static void RaiseNightBegin()                            => OnNightBegin?.Invoke();
    public static void RaiseDayAdvanced()                           => OnDayAdvanced?.Invoke();
    public static void RaiseMonsterDied(int _gold = 0)              => OnMonsterDied?.Invoke(_gold);
    public static void RaiseBuildingDestroyed(BuildingHealth _b)    => OnBuildingDestroyed?.Invoke(_b);
    public static void RaiseMainBuildingHPChanged(int _cur, int _max) => OnMainBuildingHPChanged?.Invoke(_cur, _max);
}
