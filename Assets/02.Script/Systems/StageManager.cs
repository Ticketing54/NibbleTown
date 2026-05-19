using UnityEngine;

public enum StagePhase { Day, Night }

public class StageManager : MonoBehaviour
{
    public int        CurrentDay   { get; private set; }
    public StagePhase CurrentPhase { get; private set; }

    public string StageLabel => $"{CurrentDay}일 {(CurrentPhase == StagePhase.Day ? "낮" : "밤")}";

    private void OnEnable()
    {
        GameEvents.OnDayBegin   += OnDayBegin;
        GameEvents.OnNightBegin += OnNightBegin;
    }

    private void OnDisable()
    {
        GameEvents.OnDayBegin   -= OnDayBegin;
        GameEvents.OnNightBegin -= OnNightBegin;
    }

    private void OnDayBegin()
    {
        if (CurrentPhase == StagePhase.Night)
        {
            CurrentDay++;
            GameEvents.RaiseDayAdvanced();
        }
        else
        {
            CurrentDay = 1;
        }

        CurrentPhase = StagePhase.Day;
        GameEvents.RaisePhaseChanged(CurrentDay, CurrentPhase);
    }

    private void OnNightBegin()
    {
        CurrentPhase = StagePhase.Night;
        GameEvents.RaisePhaseChanged(CurrentDay, CurrentPhase);
    }
}
