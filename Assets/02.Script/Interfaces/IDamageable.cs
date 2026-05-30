public interface IDamageable
{
    void TakeDamage(DamageInfo _info);
    bool IsDead { get; }
}
