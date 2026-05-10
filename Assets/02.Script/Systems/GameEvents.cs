using System;

public static class GameEvents
{
    public static event Action<CharacterData> OnCharacterDataChanged;
    public static event Action<int, int>      OnAPChanged;
    public static event Action<int>           OnLevelUp;
    public static event Action                OnAPNotEnough;
    public static event Action<int>           OnDayChanged;
    public static event Action<bool>          OnProgressBarShow;
    public static event Action<float>         OnProgressBarSetProgress;
    public static event Action<bool, string>  OnInteractionTextShow;

    public static void RaiseCharacterDataChanged(CharacterData _data) => OnCharacterDataChanged?.Invoke(_data);
    public static void RaiseAPChanged(int _current, int _max)         => OnAPChanged?.Invoke(_current, _max);
    public static void RaiseLevelUp(int _level)                        => OnLevelUp?.Invoke(_level);
    public static void RaiseAPNotEnough()                              => OnAPNotEnough?.Invoke();
    public static void RaiseDayChanged(int _day)                       => OnDayChanged?.Invoke(_day);
    public static void RaiseProgressBarShow(bool _visible)             => OnProgressBarShow?.Invoke(_visible);
    public static void RaiseProgressBarSetProgress(float _value)       => OnProgressBarSetProgress?.Invoke(_value);
    public static void RaiseInteractionTextShow(bool _visible, string _text) => OnInteractionTextShow?.Invoke(_visible, _text);
}
