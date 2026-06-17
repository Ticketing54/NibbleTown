using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    private int      itemId;
    private int      count;
    private Collider col;

    private void Awake()
    {
        col         = GetComponent<Collider>();
        col.enabled = false;
    }

    public void Init(int _itemId, int _count)
    {
        itemId = _itemId;
        count  = _count;
        StartCoroutine(CoArcDrop());
    }

    private IEnumerator CoArcDrop()
    {
        Vector3 start  = transform.position;
        Vector2 dir2D  = Random.insideUnitCircle.normalized;
        float endYPose = 6f;
        float   dist   = Random.Range(0.8f, 2f);
        Vector3 end = new Vector3(start.x + dir2D.x * dist, endYPose + 0.5f, start.z + dir2D.y * dist);
        float   duration = Random.Range(0.4f, 0.7f);
        float   height   = Random.Range(1f, 2f);
        float   elapsed  = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float   t   = Mathf.Clamp01(elapsed / duration);
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y              = Mathf.Lerp(start.y, end.y, t) + height * 4f * t * (1f - t);
            transform.position = pos;
            yield return null;
        }

        transform.position = end;
        col.isTrigger      = true;
        col.enabled        = true;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.TryGetComponent<Inventory>(out var inv)) return;
        col.enabled = false;
        StartCoroutine(CoMoveToPlayer(_other.transform, inv));
    }

    private IEnumerator CoMoveToPlayer(Transform target, Inventory inv)
    {
        float speed = 6f;

        while (target != null)
        {
            speed             += Time.deltaTime * 10f;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.3f)
            {
                if (inv.AddItem(itemId, count))
                    ResourceSpawnManager.Instance?.ReturnPickup(gameObject);
                yield break;
            }

            yield return null;
        }
    }
}
