using System;
using System.Collections.Generic;
using UnityEngine;

public enum HitBoxMode { Overlap, Trigger }

public class HitBox : MonoBehaviour, IAttackBehavior
{
    [SerializeField] private HitBoxMode mode          = HitBoxMode.Overlap;
    [SerializeField] private float      overlapRadius  = 1f;
    [SerializeField] private LayerMask  targetLayer;

    public event Action<IDamageable> OnHit;

    private int                  damage;
    private GameObject           dealer;
    private HashSet<IDamageable> hitTargets = new();

    public void Attack(int _damage, GameObject _dealer) => Activate(_damage, _dealer);

    // 애니메이션 이벤트 또는 스킬에서 호출
    public void Activate(int _damage, GameObject _dealer)
    {
        damage = _damage;
        dealer = _dealer;
        hitTargets.Clear();

        if (mode == HitBoxMode.Overlap)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, overlapRadius, targetLayer);
            foreach (Collider col in hits)
                TryHit(col.GetComponentInParent<IDamageable>());
        }
    }

    // Trigger 모드: 콜라이더 활성화 전 damage 세팅용
    public void SetDamage(int _damage, GameObject _dealer)
    {
        damage = _damage;
        dealer = _dealer;
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (mode != HitBoxMode.Trigger) return;
        TryHit(_other.GetComponentInParent<IDamageable>());
    }

    private void TryHit(IDamageable _target)
    {
        if (_target == null || _target.IsDead) return;
        if (!hitTargets.Add(_target)) return;
        _target.TakeDamage(new DamageInfo(dealer, damage));
        OnHit?.Invoke(_target);
    }

    private void OnDrawGizmosSelected()
    {
        if (mode != HitBoxMode.Overlap) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, overlapRadius);
    }
}
