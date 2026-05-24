using UnityEngine;

public enum StagePhase { Day, Night }

public class StageManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public int        CurrentDay   { get; private set; }
    public StagePhase CurrentPhase { get; private set; }

    public string StageLabel => $"{CurrentDay}일 {(CurrentPhase == StagePhase.Day ? "낮" : "밤")}";

    private int aliveCount;

    private void OnEnable()
    {
        GameEvents.OnDayBegin   += OnDayBegin;
        GameEvents.OnNightBegin += OnNightBegin;
        GameEvents.OnMonsterDied += OnMonsterDied;
    }

    private void OnDisable()
    {
        GameEvents.OnDayBegin    -= OnDayBegin;
        GameEvents.OnNightBegin  -= OnNightBegin;
        GameEvents.OnMonsterDied -= OnMonsterDied;
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
        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        StageData stage = GameDataManager.GetStage(CurrentDay);
        if (stage == null) return;

        aliveCount = 0;
        int spawnIndex = 0;
        foreach (StageMonsterEntry entry in stage.monsters)
            Spawn(entry.monsterIndex, entry.count, ref spawnIndex);
    }

    private void Spawn(int _monsterIndex, int _count, ref int _spawnIndex)
    {
        if (_count <= 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[StageManager] spawnPoints가 비어있습니다.");
            return;
        }

        MonsterData data = GameDataManager.GetMonster(_monsterIndex);
        if (data?.prefab == null) return;

        aliveCount += _count;
        for (int i = 0; i < _count; i++, _spawnIndex++)
        {
            Transform point = spawnPoints[_spawnIndex % spawnPoints.Length];
            Instantiate(data.prefab, point.position, point.rotation);
        }
    }

    private void OnMonsterDied()
    {
        aliveCount--;
        if (aliveCount <= 0)
            GameEvents.RaiseNextDayRequested();
    }
}
