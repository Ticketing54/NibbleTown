using System;

public interface IWeaponState
{
    WeaponType EquippedWeaponType { get; }
    event Action OnAttackTriggered;
}
