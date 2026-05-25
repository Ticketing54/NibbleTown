using System;
using System.Collections.Generic;
using UnityEngine;

public enum HitZoneMode { Overlap, Trigger }

public class HitZone : MonoBehaviour
{
    [SerializeField] private HitZoneMode mode          = HitZoneMode.Overlap;
    [SerializeField] private float       overlapRadius  = 1f;
    [SerializeField] private LayerMask   targetLayer;

    public event Action<IDamageable> OnHit;

    private int                   damage;
    private HashSet<IDamageable>  hitTargets = new();

    // 애니메이션 이벤트 또는 스킬에서 호출
    public void Activate(int _damage)
    {
        damage = _damage;
        hitTargets.Clear();

        if (mode == HitZoneMode.Overlap)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, overlapRadius, targetLayer);
            foreach (Collider col in hits)
                TryHit(col.GetComponent<IDamageable>());
        }
    }

    // Trigger 모드: 콜라이더 활성화 전 damage 세팅용
    public void SetDamage(int _damage)
    {
        damage = _damage;
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (mode != HitZoneMode.Trigger) return;
        TryHit(_other.GetComponent<IDamageable>());
    }

    private void TryHit(IDamageable _target)
    {
        if (_target == null || _target.IsDead) return;
        if (!hitTargets.Add(_target)) return;

        _target.TakeDamage(damage);
        OnHit?.Invoke(_target);
    }

    private void OnDrawGizmosSelected()
    {
        if (mode != HitZoneMode.Overlap) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, overlapRadius);
    }
}
