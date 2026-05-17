using System.Collections.Generic;

public static class CsvParser
{
    public static List<string[]> Parse(string _text)
    {
        var result = new List<string[]>();
        var lines  = _text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim('\r', ' ');
            if (string.IsNullOrEmpty(line)) continue;
            result.Add(line.Split(','));
        }
        return result;
    }

    public static int   Int(string _s)   => int.TryParse(_s.Trim(), out int v)     ? v : 0;
    public static float Float(string _s) => float.TryParse(_s.Trim(), out float v) ? v : 0f;
    public static bool  Bool(string _s)  => _s.Trim().ToLower() == "true";
}
