using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserDataManager
{
    public static int Score { get; private set; }
    public static IReadOnlyList<string> Achievements => achievements;

    private static readonly List<string> achievements = new List<string>();

    private static string SavePath => Path.Combine(Application.persistentDataPath, "userdata.csv");

    public static void Init()
    {
        Load();
    }

    // ── 점수 ──────────────────────────────────────────────────

    public static void AddScore(int _amount)
    {
        Score += _amount;
        Save();
    }

    public static void ResetScore()
    {
        Score = 0;
        Save();
    }

    // ── 업적 ──────────────────────────────────────────────────

    public static void UnlockAchievement(string _id)
    {
        if (achievements.Contains(_id)) return;
        achievements.Add(_id);
        Save();
        Debug.Log($"[UserDataManager] 업적 해금: {_id}");
    }

    public static bool HasAchievement(string _id) => achievements.Contains(_id);

    // ── 저장 / 로드 ───────────────────────────────────────────

    private static void Load()
    {
        Score = 0;
        achievements.Clear();

        if (!File.Exists(SavePath))
        {
            Debug.Log("[UserDataManager] 저장 파일 없음 — 초기값으로 시작");
            return;
        }

        foreach (var line in File.ReadAllLines(SavePath))
        {
            var cols = line.Split(',');
            if (cols.Length < 2) continue;

            string type  = cols[0].Trim();
            string value = cols[1].Trim();

            if (type == "score" && int.TryParse(value, out int s))
                Score = s;
            else if (type == "achievement")
                achievements.Add(value);
        }

        Debug.Log($"[UserDataManager] 로드 완료 — 점수: {Score}, 업적: {achievements.Count}개");
    }

    private static void Save()
    {
        var lines = new List<string> { $"score,{Score}" };

        foreach (var id in achievements)
            lines.Add($"achievement,{id}");

        File.WriteAllLines(SavePath, lines);
    }
}
