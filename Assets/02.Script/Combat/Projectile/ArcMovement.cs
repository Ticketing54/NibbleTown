using UnityEngine;

public class ArcMovement : ProjectileMovement
{
    [SerializeField] private float speed     = 8f;
    [SerializeField] private float arcHeight = 3f;
    [SerializeField] private float range     = 10f;

    private Vector3 startPos;
    private Vector3 endPos;
    private float   duration;
    private float   elapsed;

    public override void Init(Vector3 _direction, Transform _target = null)
    {
        startPos = transform.position;
        endPos   = _target != null
            ? _target.position
            : startPos + _direction.normalized * range;

        duration = Vector3.Distance(startPos, endPos) / speed;
        elapsed  = 0f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        Vector3 linear = Vector3.Lerp(startPos, endPos, t);
        Vector3 newPos = linear + Vector3.up * (arcHeight * Mathf.Sin(Mathf.PI * t));

        Vector3 dir = (newPos - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        transform.position = newPos;

        if (t >= 1f) Destroy(gameObject);
    }
}
