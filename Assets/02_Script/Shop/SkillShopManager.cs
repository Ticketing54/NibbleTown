using System.Collections.Generic;
using UnityEngine;

public class SkillShopManager : MonoBehaviour, IShopManager, INpcConversationHandler
{
    public IReadOnlyList<int> CurrentEntries => currentSkills;
    public IReadOnlyList<int> CurrentSkills  => currentSkills;

    private readonly List<int> currentSkills = new();
    private int currentDay = 1;

    private void OnEnable()
    {
        GameEvents.OnDayChanged  += OnDayChanged;
        GameEvents.OnDayBegin    += Refresh;
        GameEvents.OnDayAdvanced += Refresh;
    }

    private void OnDisable()
    {
        GameEvents.OnDayChanged  -= OnDayChanged;
        GameEvents.OnDayBegin    -= Refresh;
        GameEvents.OnDayAdvanced -= Refresh;
    }

    public void OnConversationStarted() => GameEvents.RaiseSkillShopOpen(currentSkills);
    public void OnConversationEnded()   => GameEvents.RaiseSkillShopClose();

    private void OnDayChanged(int day) => currentDay = day;

    private void Refresh()
    {
        currentSkills.Clear();

        var data = GameDataManager.SkillShop;
        if (data == null || data.tiers.Count == 0) return;

        var pool  = BuildPool(data);
        int count = Mathf.Min(data.slotCount, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int pick = Random.Range(0, pool.Count);
            currentSkills.Add(pool[pick].skillId);
            pool.RemoveAt(pick);
        }
    }

    private List<SkillShopData.Entry> BuildPool(SkillShopData data)
    {
        var pool = new List<SkillShopData.Entry>();
        foreach (var tier in data.tiers)
            if (tier.unlockAtDay <= currentDay)
                pool.AddRange(tier.skills);
        return pool;
    }
}
