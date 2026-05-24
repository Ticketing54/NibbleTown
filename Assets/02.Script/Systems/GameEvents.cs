using System;
using System.Collections.Generic;

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
    public static event Action<int, int>                              OnInventoryChanged;
    public static event Action                                        OnInventoryRefreshRequested;
    public static event Action<IReadOnlyDictionary<int, int>>         OnInventoryRefreshed;

    // ── 플레이어 입력 ──────────────────────────────────────────────
    public static event Action<bool> OnPlayerInputLocked;

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
    public static void RaiseInventoryChanged(int _itemId, int _count)                        => OnInventoryChanged?.Invoke(_itemId, _count);
    public static void RaiseInventoryRefreshRequested()                                       => OnInventoryRefreshRequested?.Invoke();
    public static void RaiseInventoryRefreshed(IReadOnlyDictionary<int, int> _items)         => OnInventoryRefreshed?.Invoke(_items);

    public static void RaisePlayerInputLocked(bool _locked) => OnPlayerInputLocked?.Invoke(_locked);

    public static event Action OnMonsterDied;

    public static void RaiseNightRequested()   => OnNightRequested?.Invoke();
    public static void RaiseNextDayRequested() => OnNextDayRequested?.Invoke();
    public static void RaiseDayBegin()         => OnDayBegin?.Invoke();
    public static void RaiseNightBegin()       => OnNightBegin?.Invoke();
    public static void RaiseDayAdvanced()      => OnDayAdvanced?.Invoke();
    public static void RaiseMonsterDied()      => OnMonsterDied?.Invoke();
}
