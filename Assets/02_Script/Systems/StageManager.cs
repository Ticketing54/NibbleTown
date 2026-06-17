using UnityEngine;
using UnityEngine.AI;

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
        GameEvents.RaiseDayChanged(CurrentDay);
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
            Spawn(entry.monsterIndex, entry.count, stage.dropTable, ref spawnIndex);
    }

    private void Spawn(int _monsterIndex, int _count, DropTable _dropTable, ref int _spawnIndex)
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
            Transform point    = spawnPoints[_spawnIndex % spawnPoints.Length];
            Vector3   spawnPos = point.position;

            if (NavMesh.SamplePosition(point.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                spawnPos = hit.position;
            else
                Debug.LogWarning($"[StageManager] 스폰 포인트 {_spawnIndex % spawnPoints.Length} 근처에 NavMesh가 없습니다.");

            GameObject go = Instantiate(data.prefab, spawnPos, point.rotation);
            go.GetComponent<MonsterController>().Init(data, _dropTable);
        }
    }

    private void OnMonsterDied(int _gold)
    {
        aliveCount--;
        if (aliveCount <= 0)
            GameEvents.RaiseNextDayRequested();
    }
}
