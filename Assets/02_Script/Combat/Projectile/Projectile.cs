using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float      lifetime    = 5f;
    [SerializeField] private LayerMask  targetLayer;
    [SerializeField] private GameObject onHitPrefab;

    private int        damage;
    private GameObject dealer;
    private float      maxDistance  = float.MaxValue;
    private Vector3    spawnPos;

    public void Init(int _damage, GameObject _dealer, Vector3 _direction, float _maxDistance = float.MaxValue, Transform _target = null)
    {
        damage      = _damage;
        dealer      = _dealer;
        maxDistance = _maxDistance;
        spawnPos    = transform.position;
        Destroy(gameObject, lifetime);

        GetComponent<ProjectileMovement>()?.Init(_direction, _target);
    }

    private void Update()
    {
        if (maxDistance < float.MaxValue &&
            Vector3.Distance(transform.position, spawnPos) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (targetLayer != 0 && (targetLayer & (1 << _other.gameObject.layer)) == 0) return;

        IDamageable damageable = _other.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead) return;

        damageable.TakeDamage(new DamageInfo(dealer, damage));

        if (onHitPrefab != null)
            Instantiate(onHitPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
