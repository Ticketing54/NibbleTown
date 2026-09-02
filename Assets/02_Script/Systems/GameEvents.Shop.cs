using System;
using System.Collections.Generic;

public static partial class GameEvents
{
    // ── 상점 ─────────────────────────────────────────────────────
    public static event Action<IReadOnlyList<int>> OnShopOpen;
    public static event Action                     OnShopClose;
    public static event Action<IReadOnlyList<int>> OnSkillShopOpen;
    public static event Action                     OnSkillShopClose;

    public static void RaiseShopOpen(IReadOnlyList<int> _items)      => OnShopOpen?.Invoke(_items);
    public static void RaiseShopClose()                              => OnShopClose?.Invoke();
    public static void RaiseSkillShopOpen(IReadOnlyList<int> _skills) => OnSkillShopOpen?.Invoke(_skills);
    public static void RaiseSkillShopClose()                          => OnSkillShopClose?.Invoke();
}
