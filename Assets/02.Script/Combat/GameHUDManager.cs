using System.Collections.Generic;
using UnityEngine;

public class GameHUDManager : MonoBehaviour
{
    public static GameHUDManager Instance { get; private set; }

    [Header("HP Bar")]
    [SerializeField] private MonsterInfoUI hpPrefab;
    [SerializeField] private int           hpPrefabPoolSize = 20;

    [Header("Damage Number")]
    [SerializeField] private DamageNumber damageNumberPrefab;
    [SerializeField] private int          damageNumberPoolSize = 20;

    private readonly Queue<MonsterInfoUI>              hudPool    = new();
    private readonly Dictionary<IHasHP, MonsterInfoUI> activeHUDs = new();
    private readonly Queue<DamageNumber>               dmgPool    = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < hpPrefabPoolSize; i++)
            hudPool.Enqueue(CreatePooled(hpPrefab));

        for (int i = 0; i < damageNumberPoolSize; i++)
            dmgPool.Enqueue(CreatePooled(damageNumberPrefab));
    }

    // ── HP Bar ───────────────────────────────────────────────────────────────

    public void Register(IHasHP _target, Transform _anchor, string _name)
    {
        if (hudPool.Count == 0) return;

        MonsterInfoUI ui = hudPool.Dequeue();
        ui.gameObject.SetActive(true);
        ui.Init(_target, _anchor, _name);
        activeHUDs[_target] = ui;

        _target.OnDied += () => Unregister(_target);
    }

    public void Unregister(IHasHP _target)
    {
        if (!activeHUDs.TryGetValue(_target, out MonsterInfoUI ui)) return;

        ui.Hide();
        ui.Cleanup();
        ui.gameObject.SetActive(false);
        activeHUDs.Remove(_target);
        hudPool.Enqueue(ui);
    }

    public void ShowHUD(IHasHP _target)
    {
        if (activeHUDs.TryGetValue(_target, out MonsterInfoUI ui))
            ui.Show();
    }

    public void HideHUD(IHasHP _target)
    {
        if (activeHUDs.TryGetValue(_target, out MonsterInfoUI ui))
            ui.Hide();
    }

    // ── Damage Number ────────────────────────────────────────────────────────

    public void SpawnDamageNumber(int _amount, Vector3 _position, bool _isCrit)
    {
        DamageNumber dn = dmgPool.Count > 0 ? dmgPool.Dequeue() : CreatePooled(damageNumberPrefab);
        dn.gameObject.SetActive(true);
        dn.Play(_amount, _position, _isCrit, () =>
        {
            dn.gameObject.SetActive(false);
            dmgPool.Enqueue(dn);
        });
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private T CreatePooled<T>(T _prefab) where T : MonoBehaviour
    {
        T instance = Instantiate(_prefab, transform);
        instance.gameObject.SetActive(false);
        return instance;
    }
}
