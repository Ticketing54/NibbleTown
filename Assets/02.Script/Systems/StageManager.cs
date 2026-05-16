using UnityEngine;

public enum StagePhase { Day, Night }

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] private CharacterStat characterStat;

    public int        CurrentDay   { get; private set; } = 1;
    public StagePhase CurrentPhase { get; private set; } = StagePhase.Day;

    public string StageLabel => $"{CurrentDay}일 {(CurrentPhase == StagePhase.Day ? "낮" : "밤")}";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AdvanceToNight()
    {
        if (CurrentPhase == StagePhase.Night) return;

        CurrentPhase = StagePhase.Night;
        GameEvents.RaisePhaseChanged(CurrentDay, CurrentPhase);
    }

    public void AdvanceToNextDay()
    {
        if (CurrentPhase == StagePhase.Day) return;

        CurrentDay++;
        CurrentPhase = StagePhase.Day;
        characterStat?.AdvanceDay();
        GameEvents.RaisePhaseChanged(CurrentDay, CurrentPhase);
    }
}
