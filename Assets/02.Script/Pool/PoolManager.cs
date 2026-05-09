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

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
    private readonly Dictionary<GameObject, GameObject>             _keyMap = new();

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

    public GameObject Get(GameObject prefab)
    {
        var pool     = EnsurePool(prefab);
        var instance = pool.Get();
        _keyMap[instance] = prefab;
        return instance;
    }

    public T Get<T>(GameObject prefab) where T : Component
    {
        return Get(prefab).GetComponent<T>();
    }

    public void Release(GameObject instance)
    {
        if (!_keyMap.TryGetValue(instance, out var key))
        {
            Debug.LogWarning($"[PoolManager] {instance.name}은 이 풀에서 꺼낸 오브젝트가 아닙니다.");
            Destroy(instance);
            return;
        }
        _pools[key].Release(instance);
        _keyMap.Remove(instance);
    }

    // ── Internal ─────────────────────────────────────────────────

    private ObjectPool<GameObject> EnsurePool(GameObject prefab, int initial = 10, int max = 100)
    {
        if (_pools.TryGetValue(prefab, out var existing))
            return existing;

        var pool = new ObjectPool<GameObject>(
            createFunc:      ()  => CreateInstance(prefab),
            actionOnGet:     go  => OnGet(go),
            actionOnRelease: go  => OnRelease(go),
            actionOnDestroy: go  => Destroy(go),
            collectionCheck: false,
            defaultCapacity: initial,
            maxSize:         max
        );

        // Pre-warm: initial 개수만큼 미리 생성 후 반환
        var warmUp = new GameObject[initial];
        for (int i = 0; i < initial; i++) warmUp[i] = pool.Get();
        for (int i = 0; i < initial; i++) pool.Release(warmUp[i]);

        _pools[prefab] = pool;
        return pool;
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        var go = Instantiate(prefab, transform);
        go.SetActive(false);
        return go;
    }

    private static void OnGet(GameObject go)
    {
        go.SetActive(true);
        go.GetComponent<IPoolable>()?.OnGetFromPool();
    }

    private static void OnRelease(GameObject go)
    {
        go.GetComponent<IPoolable>()?.OnReleaseToPool();
        go.SetActive(false);
    }
}
