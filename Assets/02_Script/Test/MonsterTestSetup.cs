using System.Collections;
using UnityEngine;

public class MonsterTestSetup : MonoBehaviour
{
    [Header("몬스터 설정")]
    [SerializeField] private MonsterData testData;

    [Header("타겟")]
    [SerializeField] private CharacterStat target;

    private void Start()
    {
        StartCoroutine(CoSetup());
    }

    private IEnumerator CoSetup()
    {
        // CharacterStat이 PlayerRegistry에 등록될 시간 확보
        yield return null;

        MonsterController[] monsters = FindObjectsByType<MonsterController>(FindObjectsSortMode.None);
        foreach (MonsterController mc in monsters)
        {
            if (testData != null)
                mc.Init(testData, null);
        }

        GameEvents.RaiseDayBegin();
        GameEvents.RaiseNightBegin();

        Debug.Log($"[MonsterTest] 몬스터 {monsters.Length}마리 초기화 완료");
    }
}
