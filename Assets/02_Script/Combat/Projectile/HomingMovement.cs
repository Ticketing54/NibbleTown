using UnityEngine;

public class HomingMovement : ProjectileMovement
{
    [SerializeField] private float speed     = 10f;
    [SerializeField] private float turnSpeed = 5f;

    private Transform target;
    private Vector3   direction;

    public override void Init(Vector3 _direction, Transform _target = null)
    {
        direction = _direction.normalized;
        target    = _target;
    }

    private void Update()
    {
        if (target != null)
        {
            IDamageable d = target.GetComponentInParent<IDamageable>();
            if (d != null && !d.IsDead)
            {
                Vector3 toTarget = (target.position - transform.position).normalized;
                direction = Vector3.Slerp(direction, toTarget, turnSpeed * Time.deltaTime);
            }
        }

        transform.position += direction * speed * Time.deltaTime;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
