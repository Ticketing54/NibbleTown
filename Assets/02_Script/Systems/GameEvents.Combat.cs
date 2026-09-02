using System;
using UnityEngine;

public static partial class GameEvents
{
    // ── 전투 ─────────────────────────────────────────────────────
    public static event Action<int>            OnMonsterDied;
    public static event Action<BuildingHealth> OnBuildingDestroyed;
    public static event Action<int, int>       OnMainBuildingHPChanged;

    public static void RaiseMonsterDied(int _gold = 0)               => OnMonsterDied?.Invoke(_gold);
    public static void RaiseBuildingDestroyed(BuildingHealth _b)     => OnBuildingDestroyed?.Invoke(_b);
    public static void RaiseMainBuildingHPChanged(int _cur, int _max) => OnMainBuildingHPChanged?.Invoke(_cur, _max);

    // ── HUD ───────────────────────────────────────────────────────
    public static event Action<IHasHP, Transform, string> OnHUDRegisterRequested;
    public static event Action<IHasHP>                    OnHUDShowRequested;
    public static event Action<IHasHP>                    OnHUDHideRequested;
    public static event Action<int, Vector3, bool>        OnDamageNumberRequested;

    public static void RaiseHUDRegister(IHasHP target, Transform anchor, string name) => OnHUDRegisterRequested?.Invoke(target, anchor, name);
    public static void RaiseHUDShow(IHasHP target)                                    => OnHUDShowRequested?.Invoke(target);
    public static void RaiseHUDHide(IHasHP target)                                    => OnHUDHideRequested?.Invoke(target);
    public static void RaiseDamageNumber(int amount, Vector3 worldPos, bool isCrit)   => OnDamageNumberRequested?.Invoke(amount, worldPos, isCrit);
}
