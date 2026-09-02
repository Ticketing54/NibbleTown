using System;

public static partial class GameEvents
{
    // ── 페이즈 전환 ───────────────────────────────────────────────
    public static event Action<int, StagePhase> OnPhaseChanged;
    public static event Action                  OnNightRequested;
    public static event Action                  OnNextDayRequested;
    public static event Action                  OnDayBegin;
    public static event Action                  OnNightBegin;
    public static event Action                  OnDayAdvanced;

    public static void RaisePhaseChanged(int _day, StagePhase _phase) => OnPhaseChanged?.Invoke(_day, _phase);
    public static void RaiseNightRequested()                           => OnNightRequested?.Invoke();
    public static void RaiseNextDayRequested()                         => OnNextDayRequested?.Invoke();
    public static void RaiseDayBegin()                                 => OnDayBegin?.Invoke();
    public static void RaiseNightBegin()                               => OnNightBegin?.Invoke();
    public static void RaiseDayAdvanced()                              => OnDayAdvanced?.Invoke();
}
