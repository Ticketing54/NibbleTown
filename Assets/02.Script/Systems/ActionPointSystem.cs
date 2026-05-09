using System;
using UnityEngine;

public class ActionPointSystem : MonoBehaviour, IActionPoints
{
    public static ActionPointSystem Instance { get; private set; }

    [SerializeField] private CharacterStatConfig config;

    // 런타임 상태 — 저장 대상
    private CharacterData _data = new CharacterData();
    private bool _lowFired;

    // IActionPoints
    public int Current => _data.currentAP;
    public int Max     => _data.maxAP;
    public int Level   => _data.level;

    public event Action<int, int> OnAPChanged;
    public event Action<int>      OnLevelUp;
    public event Action           OnAPLow;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (config == null)
            Debug.LogWarning("[ActionPointSystem] CharacterStatConfig이 연결되지 않음 — 기본값 사용");

        _data.maxAP    = config != null ? config.GetMaxAP(1) : 10;
        _data.currentAP = _data.maxAP;
    }

    // ── 저장/불러오기 인터페이스 ──────────────────────────

    public CharacterData GetSaveData()            => _data;

    public void LoadSaveData(CharacterData data)
    {
        _data     = data;
        _lowFired = false;
        OnAPChanged?.Invoke(Current, Max);
    }

    // ── IActionPoints ────────────────────────────────────

    public bool CanSpend(int amount) => _data.currentAP >= amount;

    public bool TrySpend(int amount)
    {
        if (!CanSpend(amount)) return false;

        _data.currentAP    -= amount;
        _data.totalConsumed += amount;

        CheckLevelUp();
        OnAPChanged?.Invoke(Current, Max);
        CheckLow();
        return true;
    }

    public void Restore()
    {
        _data.currentAP = _data.maxAP;
        _lowFired       = false;
        OnAPChanged?.Invoke(Current, Max);
    }

    // ── 내부 ─────────────────────────────────────────────

    private void CheckLevelUp()
    {
        int newLevel = config != null
            ? config.GetLevel(_data.totalConsumed)
            : 1 + _data.totalConsumed / 20;

        if (newLevel <= _data.level) return;

        _data.level  = newLevel;
        _data.maxAP  = config != null ? config.GetMaxAP(newLevel) : 10 + (newLevel - 1) * 2;
        _data.currentAP = Mathf.Min(_data.currentAP, _data.maxAP);
        OnLevelUp?.Invoke(_data.level);
    }

    private void CheckLow()
    {
        if (_lowFired) return;
        float threshold = config != null ? config.lowThreshold : 0.1f;
        if ((float)_data.currentAP / _data.maxAP < threshold)
        {
            _lowFired = true;
            OnAPLow?.Invoke();
        }
    }
}
