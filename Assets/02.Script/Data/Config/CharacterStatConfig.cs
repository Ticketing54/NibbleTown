using UnityEngine;

public enum WeaponType { Unarmed = 0, Sword = 1, Axe = 2, Bow = 3, Spear = 4 }

[CreateAssetMenu(fileName = "CharacterStatConfig", menuName = "NibbleTown/Character Stat Config")]
public class CharacterStatConfig : ScriptableObject
{
    [Header("HP")]
    public int baseMaxHP = 100;

    [Header("MP")]
    public int baseMaxMP = 50;

    [Header("행동력")]
    public int baseMaxAP = 10;

    [Header("AP 부족 경고 임계값 (비율)")]
    [Range(0f, 0.3f)] public float lowThreshold = 0.1f;

    [Header("전투")]
    public WeaponType weaponType     = WeaponType.Unarmed;
    public int        attackDamage   = 10;
    public float      attackRange    = 1.5f;
    public float      attackCooldown = 1f;

    [Header("치명타")]
    [Range(0f, 1f)] public float critChance      = 0.1f;
    [Range(0f, 5f)] public float critBonusRate   = 0.5f;  // 0.5 = +50% 추가 피해
}
