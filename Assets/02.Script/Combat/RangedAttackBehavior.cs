using System.Collections;
using UnityEngine;

public class RangedAttackBehavior : MonoBehaviour, IAttackBehavior
{
    public enum FirePattern { Single, Spread, Burst }

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform  firePoint;
    [SerializeField] private FirePattern pattern = FirePattern.Single;

    [Header("Spread")]
    [SerializeField] private int   projectileCount = 5;
    [SerializeField] private float spreadAngle     = 30f;

    [Header("Burst")]
    [SerializeField] private int   burstCount    = 3;
    [SerializeField] private float burstInterval = 0.15f;

    private MonsterController controller;

    private void Awake()
    {
        controller = GetComponentInParent<MonsterController>();
    }

    public void Attack(int _damage, GameObject _dealer)
    {
        switch (pattern)
        {
            case FirePattern.Single: FireSingle(_damage, _dealer, transform.forward); break;
            case FirePattern.Spread: FireSpread(_damage, _dealer);                    break;
            case FirePattern.Burst:  StartCoroutine(FireBurst(_damage, _dealer));     break;
        }
    }

    private void FireSingle(int _damage, GameObject _dealer, Vector3 _direction)
    {
        if (projectilePrefab == null) return;
        Vector3    pos      = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        float      maxDist  = controller != null ? controller.AttackRange * 1.2f : float.MaxValue;
        GameObject go       = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(_direction));
        if (go.TryGetComponent(out Projectile proj))
            proj.Init(_damage, _dealer, _direction, maxDist, controller?.CurrentTarget);
    }

    private void FireSpread(int _damage, GameObject _dealer)
    {
        float step = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;
        for (int i = 0; i < projectileCount; i++)
        {
            float   angle = -spreadAngle * 0.5f + step * i;
            Vector3 dir   = Quaternion.Euler(0, angle, 0) * transform.forward;
            FireSingle(_damage, _dealer, dir);
        }
    }

    private IEnumerator FireBurst(int _damage, GameObject _dealer)
    {
        for (int i = 0; i < burstCount; i++)
        {
            FireSingle(_damage, _dealer, transform.forward);
            yield return new WaitForSeconds(burstInterval);
        }
    }
}
