using System.Collections.Generic;
using UnityEngine;

public static class PlayerRegistry
{
    private static readonly List<CharacterStat> players = new();
    public static IReadOnlyList<CharacterStat> All => players;

    public static void Register(CharacterStat _player)   => players.Add(_player);
    public static void Unregister(CharacterStat _player) => players.Remove(_player);

    public static CharacterStat GetNearest(Vector3 _position)
    {
        CharacterStat nearest = null;
        float         minDist = float.MaxValue;

        foreach (CharacterStat p in players)
        {
            if (p == null || p.IsDead) continue;
            float d = Vector3.Distance(_position, p.transform.position);
            if (d < minDist) { minDist = d; nearest = p; }
        }

        return nearest;
    }

    public static bool IsPlayer(GameObject _obj)
    {
        foreach (CharacterStat p in players)
            if (p != null && p.gameObject == _obj) return true;
        return false;
    }
}
