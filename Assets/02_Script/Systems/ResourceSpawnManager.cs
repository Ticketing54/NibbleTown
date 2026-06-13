using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnManager : MonoBehaviour
{
    [Serializable]
    public struct ResourceEntry
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float chance;
    }

    [Serializable]
    public struct SpawnConfig
    {
        public Transform[]     spawnPoints;
        [Min(0)] public int    minCount;
        [Min(0)] public int    maxCount;
        public ResourceEntry[] entries;
    }

    public static ResourceSpawnManager Instance { get; private set; }

    [SerializeField] private SpawnConfig[] configs;
    [SerializeField] private GameObject    itemPickupPrefab;

    private readonly List<GameObject> activeResources = new();
    private readonly List<GameObject> activePickups   = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
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

    private void OnDayBegin()
    {
        ClearList(activeResources);
        ClearList(activePickups);
        foreach (SpawnConfig cfg in configs)
            SpawnFromConfig(cfg);
    }

    private void OnNightBegin() => ClearList(activeResources);

    private void SpawnFromConfig(SpawnConfig cfg)
    {
        if (cfg.spawnPoints == null || cfg.spawnPoints.Length == 0) return;
        if (cfg.entries == null || cfg.entries.Length == 0) return;

        int count = Mathf.Clamp(
            UnityEngine.Random.Range(cfg.minCount, cfg.maxCount + 1),
            0, cfg.spawnPoints.Length);

        Transform[] pts = (Transform[])cfg.spawnPoints.Clone();
        Shuffle(pts);

        for (int i = 0; i < count; i++)
        {
            if (pts[i] == null) continue;
            GameObject prefab = PickWeighted(cfg.entries);
            if (prefab == null) continue;
            activeResources.Add(Instantiate(prefab, pts[i].position, pts[i].rotation));
        }
    }

    private static GameObject PickWeighted(ResourceEntry[] entries)
    {
        float total = 0f;
        foreach (var e in entries) total += e.chance;
        if (total <= 0f) return null;

        float roll = UnityEngine.Random.value * total;
        foreach (var e in entries)
        {
            roll -= e.chance;
            if (roll <= 0f) return e.prefab;
        }
        return entries[entries.Length - 1].prefab;
    }

    public void SpawnItemDrop(DropTable dropTable, Vector3 position)
    {
        if (dropTable == null || itemPickupPrefab == null) return;
        if (!dropTable.Roll(out int itemId, out int count)) return;

        Vector3 offset = new Vector3(
            UnityEngine.Random.Range(-0.3f, 0.3f), 0f,
            UnityEngine.Random.Range(-0.3f, 0.3f));

        GameObject go = Instantiate(itemPickupPrefab, position + offset, Quaternion.identity);
        go.GetComponent<ItemPickup>()?.Init(itemId, count);
        activePickups.Add(go);
    }

    public void ReturnPickup(GameObject pickup)
    {
        activePickups.Remove(pickup);
        Destroy(pickup);
    }

    private static void ClearList(List<GameObject> list)
    {
        foreach (GameObject go in list)
            if (go != null) Destroy(go);
        list.Clear();
    }

    private static void Shuffle(Transform[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}
