using UnityEngine;

[RequireComponent(typeof(CharacterStat))]
public class MonsterTestTarget : MonoBehaviour
{
    [SerializeField] private CharacterStatConfig config;

    private CharacterStat stat;

    private void Awake()
    {
        stat = GetComponent<CharacterStat>();
    }

    private void Start()
    {
        Debug.Log($"[MonsterTest] 타겟 HP: {stat.CurrentHP} / {stat.MaxHP}");
    }
}
