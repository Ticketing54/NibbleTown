using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillShopData", menuName = "NibbleTown/Shop/Skill Shop Data")]
public class SkillShopData : ScriptableObject
{
    [Serializable]
    public struct Entry { public int skillId; }

    [Serializable]
    public class PoolTier
    {
        [Tooltip("이 티어가 해금되는 일수")]
        public int         unlockAtDay = 1;
        public List<Entry> skills      = new List<Entry>();
    }

    [Tooltip("일수 구간별 스킬 풀 (unlockAtDay 이하인 티어가 모두 합산됨)")]
    public List<PoolTier> tiers     = new List<PoolTier>();

    [Tooltip("하루에 진열할 슬롯 수")]
    public int            slotCount = 3;
}
