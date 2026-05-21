using System.Collections.Generic;
using UnityEngine;


public static class GameDataManager
{
    public static bool IsInitialized { get; private set; }

    // ── Database ──────────────────────────────────────────────
    public static ItemDatabase            Items            { get; private set; }
    public static StageDatabase           Stages           { get; private set; }
    public static BuildingUpgradeDatabase BuildingUpgrades { get; private set; }
    public static ItemUpgradeDatabase     ItemUpgrades     { get; private set; }
    public static CharStatDatabase        CharStats        { get; private set; }

    // ── Config ────────────────────────────────────────────────
    public static CharacterStatConfig CharacterConfig { get; private set; }

    private static List<WeaponData> weapons;

    public static void Init()
    {
        if (IsInitialized) return;

        var settings = GameDataSettings.Instance;
        if (settings == null)
        {
            Debug.LogError("[GameDataManager] GameDataSettings 가 로드되지 않았습니다. Preloaded Assets를 확인하세요.");
            return;
        }

        Items            = settings.itemDatabase;
        Stages           = settings.stageDatabase;
        BuildingUpgrades = settings.buildingUpgradeDatabase;
        ItemUpgrades     = settings.itemUpgradeDatabase;
        CharStats        = settings.charStatDatabase;
        CharacterConfig  = settings.characterStatConfig;
        weapons          = settings.weapons;

        Items?.BuildLookup();
        Stages?.BuildLookup();
        BuildingUpgrades?.BuildLookup();
        ItemUpgrades?.BuildLookup();
        CharStats?.BuildLookup();

        IsInitialized = true;
        Debug.Log("[GameDataManager] 초기화 완료");
    }

    // ── Database 조회 ─────────────────────────────────────────
    public static ItemData            GetItem(int _itemId)                                   => Items?.Get(_itemId);
    public static StageData           GetStage(int _stageId)                                 => Stages?.Get(_stageId);
    public static BuildingUpgradeData GetBuildingUpgrade(int _buildingId, int _currentLevel) => BuildingUpgrades?.Get(_buildingId, _currentLevel);
    public static ItemUpgradeData     GetItemUpgrade(int _targetItemId)                      => ItemUpgrades?.Get(_targetItemId);
    public static CharStatData        GetCharStat(int _characterId)                          => CharStats?.Get(_characterId);

    // ── Config 조회 ───────────────────────────────────────────
    public static WeaponData GetWeapon(WeaponType _type)
    {
        if (weapons == null) return null;
        foreach (var w in weapons)
            if (w != null && w.weaponType == _type) return w;
        return null;
    }
}
