using UnityEngine;

public class AcceleratedMovement : ProjectileMovement
{
    [SerializeField] private float initialSpeed      = 3f;
    [SerializeField] private float finalSpeed        = 20f;
    [SerializeField] private float accelerationTime  = 0.5f;

    private Vector3 direction;
    private float   elapsed;

    public override void Init(Vector3 _direction, Transform _target = null)
    {
        direction = _direction.normalized;
        elapsed   = 0f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float speed = Mathf.Lerp(initialSpeed, finalSpeed, Mathf.Clamp01(elapsed / accelerationTime));
        transform.position += direction * speed * Time.deltaTime;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
