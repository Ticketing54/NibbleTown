using UnityEngine;

public class BuildingHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP = 200;

    private int currentHP;

    public bool IsDead => currentHP <= 0;

    private static readonly System.Collections.Generic.List<BuildingHealth> all = new();
    public static System.Collections.Generic.IReadOnlyList<BuildingHealth> All => all;

    private void Awake()    { currentHP = maxHP; all.Add(this); }
    private void OnDestroy() { all.Remove(this); }

    public void TakeDamage(DamageInfo _info)
    {
        if (IsDead) return;
        currentHP = Mathf.Max(0, currentHP - _info.amount);
        if (IsDead) OnDestroyed();
    }

    private void OnDestroyed()
    {
        GameEvents.RaiseBuildingDestroyed(this);
        Destroy(gameObject);
    }
}
