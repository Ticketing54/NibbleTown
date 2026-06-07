using UnityEngine;

public class LinearMovement : ProjectileMovement
{
    [SerializeField] private float speed = 10f;

    private Vector3 direction;

    public override void Init(Vector3 _direction, Transform _target = null)
    {
        direction = _direction.normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
