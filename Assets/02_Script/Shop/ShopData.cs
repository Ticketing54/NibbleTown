using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "NibbleTown/Shop/Shop Data")]
public class ShopData : ScriptableObject
{
    [Serializable]
    public struct Entry { public int itemId; }

    [Serializable]
    public class PoolTier
    {
        [Tooltip("이 티어가 해금되는 일수")]
        public int         unlockAtDay = 1;
        public List<Entry> items       = new List<Entry>();
    }

    [Tooltip("일수 구간별 아이템 풀 (unlockAtDay 이상인 티어가 모두 합산됨)")]
    public List<PoolTier> tiers = new List<PoolTier>();

    [Tooltip("하루에 진열할 슬롯 수")]
    public int slotCount = 6;
}
