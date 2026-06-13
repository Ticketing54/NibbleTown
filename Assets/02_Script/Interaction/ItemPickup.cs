using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    private int itemId;
    private int count;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void Init(int _itemId, int _count)
    {
        itemId = _itemId;
        count  = _count;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.TryGetComponent<Inventory>(out var inv)) return;
        if (!inv.AddItem(itemId, count)) return;
        ResourceSpawnManager.Instance?.ReturnPickup(gameObject);
    }
}
