using System.Collections.Generic;
using UnityEngine;

public static class GameDataManager
{
    public static bool IsInitialized { get; private set; }

    public static IReadOnlyDictionary<int, ItemRow>            Items            => items;
    public static IReadOnlyDictionary<int, StageRow>           Stages           => stages;
    public static IReadOnlyDictionary<int, List<BuildingUpgradeRow>> BuildingUpgrades => buildingUpgrades;
    public static IReadOnlyDictionary<int, ItemUpgradeRow>     ItemUpgrades     => itemUpgrades;
    public static IReadOnlyDictionary<int, CharStatRow>        CharStats        => charStats;

    static readonly Dictionary<int, ItemRow>                 items            = new Dictionary<int, ItemRow>();
    static readonly Dictionary<int, StageRow>                stages           = new Dictionary<int, StageRow>();
    static readonly Dictionary<int, List<BuildingUpgradeRow>> buildingUpgrades = new Dictionary<int, List<BuildingUpgradeRow>>();
    static readonly Dictionary<int, ItemUpgradeRow>          itemUpgrades     = new Dictionary<int, ItemUpgradeRow>();
    static readonly Dictionary<int, CharStatRow>             charStats        = new Dictionary<int, CharStatRow>();

    public static void Init()
    {
        if (IsInitialized) return;

        LoadItems();
        LoadStages();
        LoadBuildingUpgrades();
        LoadItemUpgrades();
        LoadCharStats();

        IsInitialized = true;
        Debug.Log("[GameDataManager] 초기화 완료");
    }

    // ── 로드 ─────────────────────────────────────────────────

    static void LoadItems()
    {
        var asset = Resources.Load<TextAsset>("Data/ItemData");
        if (asset == null) { Debug.LogWarning("[GameDataManager] ItemData 없음"); return; }

        foreach (var cols in CsvParser.Parse(asset.text))
        {
            if (cols.Length < 9) continue;
            int id = CsvParser.Int(cols[0]);
            items[id] = new ItemRow
            {
                itemId       = id,
                itemName     = cols[1].Trim(),
                itemType     = cols[2].Trim(),
                grade        = cols[3].Trim(),
                description  = cols[4].Trim(),
                maxStack     = CsvParser.Int(cols[5]),
                sellPrice    = CsvParser.Int(cols[6]),
                isDroppable  = CsvParser.Bool(cols[7]),
                isEquippable = CsvParser.Bool(cols[8]),
            };
        }
    }

    static void LoadStages()
    {
        var asset = Resources.Load<TextAsset>("Data/StageData");
        if (asset == null) { Debug.LogWarning("[GameDataManager] StageData 없음"); return; }

        foreach (var cols in CsvParser.Parse(asset.text))
        {
            if (cols.Length < 13) continue;
            int id = CsvParser.Int(cols[0]);
            stages[id] = new StageRow
            {
                stageId          = id,
                stageName        = cols[1].Trim(),
                requiredLevel    = CsvParser.Int(cols[2]),
                monsterType      = cols[3].Trim(),
                monsterCount     = CsvParser.Int(cols[4]),
                bossMonsterType  = cols[5].Trim(),
                timeLimitSec     = CsvParser.Int(cols[6]),
                rewardItem1Id    = CsvParser.Int(cols[7]),
                rewardItem1Count = CsvParser.Int(cols[8]),
                rewardItem2Id    = CsvParser.Int(cols[9]),
                rewardItem2Count = CsvParser.Int(cols[10]),
                rewardExp        = CsvParser.Int(cols[11]),
                rewardGold       = CsvParser.Int(cols[12]),
            };
        }
    }

    static void LoadBuildingUpgrades()
    {
        var asset = Resources.Load<TextAsset>("Data/BuildingUpgradeData");
        if (asset == null) { Debug.LogWarning("[GameDataManager] BuildingUpgradeData 없음"); return; }

        foreach (var cols in CsvParser.Parse(asset.text))
        {
            if (cols.Length < 12) continue;
            int buildingId = CsvParser.Int(cols[0]);
            var row = new BuildingUpgradeRow
            {
                buildingId         = buildingId,
                buildingName       = cols[1].Trim(),
                currentLevel       = CsvParser.Int(cols[2]),
                nextLevel          = CsvParser.Int(cols[3]),
                requiredItem1Id    = CsvParser.Int(cols[4]),
                requiredItem1Count = CsvParser.Int(cols[5]),
                requiredItem2Id    = CsvParser.Int(cols[6]),
                requiredItem2Count = CsvParser.Int(cols[7]),
                requiredItem3Id    = CsvParser.Int(cols[8]),
                requiredItem3Count = CsvParser.Int(cols[9]),
                requiredGold       = CsvParser.Int(cols[10]),
                upgradeTimeSec     = CsvParser.Int(cols[11]),
            };

            if (!buildingUpgrades.ContainsKey(buildingId))
                buildingUpgrades[buildingId] = new List<BuildingUpgradeRow>();
            buildingUpgrades[buildingId].Add(row);
        }
    }

    static void LoadItemUpgrades()
    {
        var asset = Resources.Load<TextAsset>("Data/ItemUpgradeData");
        if (asset == null) { Debug.LogWarning("[GameDataManager] ItemUpgradeData 없음"); return; }

        foreach (var cols in CsvParser.Parse(asset.text))
        {
            if (cols.Length < 14) continue;
            int id = CsvParser.Int(cols[0]);
            itemUpgrades[id] = new ItemUpgradeRow
            {
                targetItemId       = id,
                targetItemName     = cols[1].Trim(),
                currentGrade       = cols[2].Trim(),
                resultItemId       = CsvParser.Int(cols[3]),
                resultItemName     = cols[4].Trim(),
                nextGrade          = cols[5].Trim(),
                requiredItem1Id    = CsvParser.Int(cols[6]),
                requiredItem1Count = CsvParser.Int(cols[7]),
                requiredItem2Id    = CsvParser.Int(cols[8]),
                requiredItem2Count = CsvParser.Int(cols[9]),
                requiredItem3Id    = CsvParser.Int(cols[10]),
                requiredItem3Count = CsvParser.Int(cols[11]),
                requiredGold       = CsvParser.Int(cols[12]),
                successRate        = CsvParser.Float(cols[13]),
            };
        }
    }

    static void LoadCharStats()
    {
        var asset = Resources.Load<TextAsset>("Data/CharacterStatData");
        if (asset == null) { Debug.LogWarning("[GameDataManager] CharacterStatData 없음"); return; }

        foreach (var cols in CsvParser.Parse(asset.text))
        {
            if (cols.Length < 14) continue;
            int id = CsvParser.Int(cols[0]);
            charStats[id] = new CharStatRow
            {
                characterId      = id,
                characterName    = cols[1].Trim(),
                characterClass   = cols[2].Trim(),
                baseHP           = CsvParser.Int(cols[3]),
                baseMana         = CsvParser.Int(cols[4]),
                baseAttack       = CsvParser.Int(cols[5]),
                baseDefense      = CsvParser.Int(cols[6]),
                baseSpeed        = CsvParser.Float(cols[7]),
                baseAttackRange  = CsvParser.Float(cols[8]),
                baseCritRate     = CsvParser.Float(cols[9]),
                baseCritDamage   = CsvParser.Float(cols[10]),
                hpPerLevel       = CsvParser.Int(cols[11]),
                attackPerLevel   = CsvParser.Int(cols[12]),
                defensePerLevel  = CsvParser.Int(cols[13]),
            };
        }
    }

    // ── 편의 조회 ─────────────────────────────────────────────

    public static ItemRow GetItem(int _itemId) =>
        items.TryGetValue(_itemId, out var row) ? row : null;

    public static StageRow GetStage(int _stageId) =>
        stages.TryGetValue(_stageId, out var row) ? row : null;

    public static BuildingUpgradeRow GetBuildingUpgrade(int _buildingId, int _currentLevel)
    {
        if (!buildingUpgrades.TryGetValue(_buildingId, out var list)) return null;
        return list.Find(r => r.currentLevel == _currentLevel);
    }

    public static ItemUpgradeRow GetItemUpgrade(int _itemId) =>
        itemUpgrades.TryGetValue(_itemId, out var row) ? row : null;

    public static CharStatRow GetCharStat(int _characterId) =>
        charStats.TryGetValue(_characterId, out var row) ? row : null;
}
