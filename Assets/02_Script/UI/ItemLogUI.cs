using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ItemLogUI : MonoBehaviour
{
    [SerializeField] private ScrollRect     scrollRect;
    [SerializeField] private ItemLogEntryUI entryPrefab;

    [SerializeField] private float displayDuration = 0.3f;
    [SerializeField] private int   defaultPoolSize = 10;

    private IObjectPool<ItemLogEntryUI> pool;

    private void Awake()
    {
        pool = new ObjectPool<ItemLogEntryUI>(
            createFunc:      CreateEntry,
            actionOnGet:     e => e.gameObject.SetActive(true),
            actionOnRelease: e => { e.gameObject.SetActive(false); e.transform.SetParent(transform, false); },
            actionOnDestroy: e => Destroy(e.gameObject),
            defaultCapacity: defaultPoolSize
        );
    }

    private void OnEnable()
    {
        GameEvents.OnInventoryChanged += OnInventoryChanged;
        GameEvents.OnGoldAcquired     += OnGoldAcquired;
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryChanged -= OnInventoryChanged;
        GameEvents.OnGoldAcquired     -= OnGoldAcquired;
    }

    private void OnInventoryChanged(int _itemId, int _delta, int _newTotal)
    {
        if (_delta <= 0) return;

        var data = GameDataManager.GetItem(_itemId);
        if (data == null) return;

        ShowEntry($"{data.itemName} x{_delta} 을(를) 획득했습니다.");
    }

    private void OnGoldAcquired(int _amount)
    {
        ShowEntry($"골드 {_amount}를 얻었습니다.");
    }

    private void ShowEntry(string _message)
    {
        var entry = pool.Get();
        entry.transform.SetParent(scrollRect.content, false);
        entry.transform.SetAsLastSibling();
        entry.Show(_message, displayDuration, ReturnEntry);

        StartCoroutine(CoScrollToBottom());
    }

    private void ReturnEntry(ItemLogEntryUI _entry) => pool.Release(_entry);

    // 레이아웃이 갱신된 후 맨 아래로 스크롤
    private IEnumerator CoScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private ItemLogEntryUI CreateEntry()
    {
        var entry = Instantiate(entryPrefab, transform, false);
        entry.gameObject.SetActive(false);
        return entry;
    }
}
