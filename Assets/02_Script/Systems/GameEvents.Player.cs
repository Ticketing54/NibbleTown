using System;

public static partial class GameEvents
{
    // ── 캐릭터 스탯 ───────────────────────────────────────────────
    public static event Action<CharacterData> OnCharacterDataChanged;
    public static event Action<int, int>      OnAPChanged;
    public static event Action<int, int>      OnHPChanged;
    public static event Action<int, int>      OnMPChanged;
    public static event Action                OnAPNotEnough;
    public static event Action<int>           OnDayChanged;

    public static void RaiseCharacterDataChanged(CharacterData _data)  => OnCharacterDataChanged?.Invoke(_data);
    public static void RaiseAPChanged(int _current, int _max)          => OnAPChanged?.Invoke(_current, _max);
    public static void RaiseHPChanged(int _current, int _max)          => OnHPChanged?.Invoke(_current, _max);
    public static void RaiseMPChanged(int _current, int _max)          => OnMPChanged?.Invoke(_current, _max);
    public static void RaiseAPNotEnough()                              => OnAPNotEnough?.Invoke();
    public static void RaiseDayChanged(int _day)                       => OnDayChanged?.Invoke(_day);

    // ── 플레이어 입력 ─────────────────────────────────────────────
    public static event Action<bool> OnPlayerInputLocked;
    public static event Action<bool> OnNpcConversationChanged;
    public static event Action       OnNpcConversationCloseRequested;

    public static void RaisePlayerInputLocked(bool _locked)        => OnPlayerInputLocked?.Invoke(_locked);
    public static void RaiseNpcConversationChanged(bool _active)   => OnNpcConversationChanged?.Invoke(_active);
    public static void RaiseNpcConversationCloseRequested()        => OnNpcConversationCloseRequested?.Invoke();
}
