using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatConfig", menuName = "NibbleTown/Character Stat Config")]
public class CharacterStatConfig : ScriptableObject
{
    [Header("행동력")]
    public int baseMaxAP = 10;

    [Header("AP 부족 경고 임계값 (비율)")]
    [Range(0f, 0.3f)] public float lowThreshold = 0.1f;

    [Header("HP")]
    public int baseMaxHP = 100;

    [Header("MP")]
    public int baseMaxMP = 50;
}
