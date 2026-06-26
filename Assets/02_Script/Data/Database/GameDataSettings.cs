using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSettings", menuName = "NibbleTown/Game Data Settings")]
public class GameDataSettings : ScriptableObject
{
    public static GameDataSettings Instance { get; private set; }

    private void OnEnable() => Instance = this;

    [Header("Database")]
    public ItemDatabase            itemDatabase;
    public SkillDatabase           skillDatabase;
    public StageDatabase           stageDatabase;
    public MonsterDatabase         monsterDatabase;
    public BuildingUpgradeDatabase buildingUpgradeDatabase;
    public ItemUpgradeDatabase     itemUpgradeDatabase;
    public ShopData                shopData;
    public SkillShopData           skillShopData;

    [Header("Config")]
    public CharacterStatConfig characterStatConfig;
}
