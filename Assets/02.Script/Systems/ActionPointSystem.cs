using System;
using UnityEngine;

public class ActionPointSystem : MonoBehaviour, IActionPoints
{
    public static ActionPointSystem Instance { get; private set; }

    [SerializeField] private CharacterStatConfig config;

    // 런타임 상태 — 저장 대상
    private CharacterData data = new CharacterData();
    private bool lowFired;

    // IActionPoints
    public int Current => data.currentAP;
    public int Max     => data.maxAP;
    public int Level   => data.level;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (config == null)
            Debug.LogWarning("[ActionPointSystem] CharacterStatConfig이 연결되지 않음 — 기본값 사용");

        data.maxAP    = config != null ? config.GetMaxAP(1) : 10;
        data.currentAP = data.maxAP;
    }

    // ── 저장/불러오기 인터페이스 ──────────────────────────

    public CharacterData GetSaveData()            => data;

    public void LoadSaveData(CharacterData _data)
    {
        data     = _data;
        lowFired = false;
    }

    // ── IActionPoints ────────────────────────────────────

    public bool CanSpend(int _amount) => data.currentAP >= _amount;

    public bool TrySpend(int _amount)
    {
        if (!CanSpend(_amount)) return false;

        data.currentAP    -= _amount;
        data.totalConsumed += _amount;

        CheckLevelUp();
        CheckLow();
        GameEvents.RaiseAPChanged(data.currentAP, data.maxAP);
        return true;
    }

    public void Restore()
    {
        data.currentAP = data.maxAP;
        lowFired       = false;
        GameEvents.RaiseAPChanged(data.currentAP, data.maxAP);
    }

    // ── 내부 ─────────────────────────────────────────────

    private void CheckLevelUp()
    {
        int newLevel = config != null
            ? config.GetLevel(data.totalConsumed)
            : 1 + data.totalConsumed / 20;

        if (newLevel <= data.level) return;

        data.level     = newLevel;
        data.maxAP     = config != null ? config.GetMaxAP(newLevel) : 10 + (newLevel - 1) * 2;
        data.currentAP = Mathf.Min(data.currentAP, data.maxAP);
        GameEvents.RaiseLevelUp(data.level);
    }

    private void CheckLow()
    {
        if (lowFired) return;
        float threshold = config != null ? config.lowThreshold : 0.1f;
        if ((float)data.currentAP / data.maxAP < threshold)
        {
            lowFired = true;
            GameEvents.RaiseAPNotEnough();
        }
    }
}
