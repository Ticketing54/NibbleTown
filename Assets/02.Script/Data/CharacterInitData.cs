public struct CharacterInitData
{
    public string id;
    public int    maxHP;
    public int    maxMP;
    public int    baseMaxAP;
    public float  lowThreshold;

    public static CharacterInitData Default => new CharacterInitData
    {
        id           = "Unknown",
        maxHP        = 100,
        maxMP        = 50,
        baseMaxAP    = 10,
        lowThreshold = 0.1f,
    };
}
