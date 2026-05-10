using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public int Day => ActionPointSystem.Instance.GetSaveData().day;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AdvanceDay()
    {
        ActionPointSystem.Instance.GetSaveData().day++;
        ActionPointSystem.Instance.Restore();
        GameEvents.RaiseDayChanged(Day);
        Debug.Log($"[DayManager] Day {Day} 시작");
    }
}
