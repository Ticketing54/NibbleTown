using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        [Min(1)] public int initialSize = 10;
        [Min(1)] public int maxSize     = 100;
    }

    [SerializeField] private List<PoolConfig> preWarmList = new();

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pools = new();
    private readonly Dictionary<GameObject, GameObject>             keyMap = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PreWarm();
    }

    private void PreWarm()
    {
        foreach (var cfg in preWarmList)
            EnsurePool(cfg.prefab, cfg.initialSize, cfg.maxSize);
    }

    // ── Public API ──────────────────────────────────────────────

    public GameObject Get(GameObject _prefab)
    {
        var pool     = EnsurePool(_prefab);
        var instance = pool.Get();
        keyMap[instance] = _prefab;
        return instance;
    }

    public T Get<T>(GameObject _prefab) where T : Component
    {
        return Get(_prefab).GetComponent<T>();
    }

    public void Release(GameObject _instance)
    {
        if (!keyMap.TryGetValue(_instance, out var key))
        {
            Debug.LogWarning($"[PoolManager] {_instance.name}은 이 풀에서 꺼낸 오브젝트가 아닙니다.");
            Destroy(_instance);
            return;
        }
        pools[key].Release(_instance);
        keyMap.Remove(_instance);
    }

    // ── Internal ─────────────────────────────────────────────────

    private ObjectPool<GameObject> EnsurePool(GameObject _prefab, int _initial = 10, int _max = 100)
    {
        if (pools.TryGetValue(_prefab, out var existing))
            return existing;

        var pool = new ObjectPool<GameObject>(
            createFunc:      ()  => CreateInstance(_prefab),
            actionOnGet:     go  => OnGet(go),
            actionOnRelease: go  => OnRelease(go),
            actionOnDestroy: go  => Destroy(go),
            collectionCheck: false,
            defaultCapacity: _initial,
            maxSize:         _max
        );

        // Pre-warm: initial 개수만큼 미리 생성 후 반환
        var warmUp = new GameObject[_initial];
        for (int i = 0; i < _initial; i++) warmUp[i] = pool.Get();
        for (int i = 0; i < _initial; i++) pool.Release(warmUp[i]);

        pools[_prefab] = pool;
        return pool;
    }

    private GameObject CreateInstance(GameObject _prefab)
    {
        var go = Instantiate(_prefab, transform);
        go.SetActive(false);
        return go;
    }

    private static void OnGet(GameObject _go)
    {
        _go.SetActive(true);
        _go.GetComponent<IPoolable>()?.OnGetFromPool();
    }

    private static void OnRelease(GameObject _go)
    {
        _go.GetComponent<IPoolable>()?.OnReleaseToPool();
        _go.SetActive(false);
    }
}
