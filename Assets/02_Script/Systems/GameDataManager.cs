using UnityEngine;

public static class GameDataManager
{
    public static bool IsInitialized { get; private set; }

    // ── Database ──────────────────────────────────────────────
    public static ItemDatabase            Items            { get; private set; }
    public static SkillDatabase           Skills           { get; private set; }
    public static StageDatabase           Stages           { get; private set; }
    public static MonsterDatabase         Monsters         { get; private set; }
    public static BuildingUpgradeDatabase BuildingUpgrades { get; private set; }
    public static ItemUpgradeDatabase     ItemUpgrades     { get; private set; }

    public static ShopData             Shop            { get; private set; }

    // ── Config ────────────────────────────────────────────────
    public static CharacterStatConfig CharacterConfig { get; private set; }


    public static void Init(GameDataSettings settings)
    {
        if (IsInitialized) return;

        if (settings == null)
        {
            Debug.LogError("[GameDataManager] GameDataSettings 가 null 입니다. GameBootstrapper의 Inspector를 확인하세요.");
            return;
        }

        Items            = settings.itemDatabase;
        Skills           = settings.skillDatabase;
        Stages           = settings.stageDatabase;
        Monsters         = settings.monsterDatabase;
        BuildingUpgrades = settings.buildingUpgradeDatabase;
        ItemUpgrades     = settings.itemUpgradeDatabase;
        Shop             = settings.shopData;
        CharacterConfig  = settings.characterStatConfig;
        Items?.BuildLookup();
        Skills?.BuildLookup();
        Monsters?.BuildLookup();
        BuildingUpgrades?.BuildLookup();
        ItemUpgrades?.BuildLookup();

        IsInitialized = true;
        Debug.Log("[GameDataManager] 초기화 완료");
    }

    // ── Database 조회 ─────────────────────────────────────────
    public static ItemData            GetItem(int _itemId)                                   => Items?.Get(_itemId);
    public static StageData           GetStage(int _stageId)                                 => Stages?.Get(_stageId);
    public static MonsterData         GetMonster(int _monsterIndex)                          => Monsters?.Get(_monsterIndex);
    public static BuildingUpgradeData GetBuildingUpgrade(int _buildingId, int _currentLevel) => BuildingUpgrades?.Get(_buildingId, _currentLevel);
    public static ItemUpgradeData     GetItemUpgrade(int _targetItemId)                      => ItemUpgrades?.Get(_targetItemId);

}
