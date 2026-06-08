using System;
using UnityEngine;

public class BuildingHealth : MonoBehaviour, IDamageable, IHasHP
{
    [SerializeField] private int    maxHP         = 200;
    [SerializeField] private string buildingName  = "건물";
    [SerializeField] private int    repairAmount  = 30;
    [SerializeField] private bool   isMainBuilding = false;

    private int        currentHP;
    private StagePhase currentPhase;

    public bool   IsDead    => currentHP <= 0;
    public int    CurrentHP => currentHP;
    public int    MaxHP     => maxHP;

    public event Action OnHit;
    public event Action OnDied;

    private static readonly System.Collections.Generic.List<BuildingHealth> all = new();
    public static System.Collections.Generic.IReadOnlyList<BuildingHealth> All => all;

    private void Awake()
    {
        currentHP = maxHP;
        all.Add(this);
    }

    private void Start()
    {
        if (isMainBuilding)
        {
            GameEvents.RaiseMainBuildingHPChanged(currentHP, maxHP);
        }
        else
        {
            GameHUDManager.Instance?.Register(this, transform, buildingName);
            GameHUDManager.Instance?.ShowHUD(this);
        }
    }

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

    private void OnDestroy() => all.Remove(this);

    private void OnDayBegin()   => currentPhase = StagePhase.Day;
    private void OnNightBegin() => currentPhase = StagePhase.Night;

    // ── 피해 ─────────────────────────────────────────────────────

    public void TakeDamage(DamageInfo _info)
    {
        if (IsDead) return;
        currentHP = Mathf.Max(0, currentHP - _info.amount);
        NotifyHPChanged();
        if (IsDead) OnDestroyed();
    }

    // ── 수리 (낮에만 가능) ────────────────────────────────────────

    public bool CanRepair => !IsDead && currentPhase == StagePhase.Day && currentHP < maxHP;

    public void Repair()
    {
        if (!CanRepair) return;
        currentHP = Mathf.Min(maxHP, currentHP + repairAmount);
        NotifyHPChanged();
    }

    private void NotifyHPChanged()
    {
        OnHit?.Invoke();
        if (isMainBuilding)
            GameEvents.RaiseMainBuildingHPChanged(currentHP, maxHP);
    }

    // ── 파괴 ─────────────────────────────────────────────────────

    private void OnDestroyed()
    {
        OnDied?.Invoke();
        GameEvents.RaiseBuildingDestroyed(this);
        Destroy(gameObject);
    }
}
