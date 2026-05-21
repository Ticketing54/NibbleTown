using UnityEngine;

public enum WeaponType { Unarmed = 0, Sword = 1, Axe = 2, Bow = 3, Spear = 4 }

[CreateAssetMenu(fileName = "WeaponData", menuName = "NibbleTown/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    [Min(0.1f)] public float attackCooldown = 1f;
}
