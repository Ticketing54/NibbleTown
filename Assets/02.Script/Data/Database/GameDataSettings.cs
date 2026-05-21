using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSettings", menuName = "NibbleTown/Game Data Settings")]
public class GameDataSettings : ScriptableObject
{
    public static GameDataSettings Instance { get; private set; }

    private void OnEnable() => Instance = this;

    [Header("Database")]
    public ItemDatabase            itemDatabase;
    public StageDatabase           stageDatabase;
    public BuildingUpgradeDatabase buildingUpgradeDatabase;
    public ItemUpgradeDatabase     itemUpgradeDatabase;
    public CharStatDatabase        charStatDatabase;

    [Header("Config")]
    public CharacterStatConfig  characterStatConfig;
    public List<WeaponData>     weapons;
}
