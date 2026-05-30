using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float     speed       = 10f;
    [SerializeField] private float     lifetime    = 5f;
    [SerializeField] private bool      homing      = false;
    [SerializeField] private LayerMask targetLayer;

    private int        damage;
    private GameObject dealer;
    private Transform  target;
    private Vector3    direction;

    public void Init(int _damage, GameObject _dealer, Vector3 _direction)
    {
        damage    = _damage;
        dealer    = _dealer;
        direction = _direction.normalized;
        Destroy(gameObject, lifetime);
    }

    public void Init(int _damage, GameObject _dealer, Transform _target)
    {
        damage    = _damage;
        dealer    = _dealer;
        target    = _target;
        direction = (_target.position - transform.position).normalized;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (homing && target != null)
        {
            IDamageable d = target.GetComponentInParent<IDamageable>();
            if (d != null && !d.IsDead)
                direction = (target.position - transform.position).normalized;
        }

        transform.position += direction * speed * Time.deltaTime;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (targetLayer != 0 && (targetLayer & (1 << _other.gameObject.layer)) == 0) return;

        IDamageable damageable = _other.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead) return;

        damageable.TakeDamage(new DamageInfo(dealer, damage));
        Destroy(gameObject);
    }
}
