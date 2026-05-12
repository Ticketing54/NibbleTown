using System;

[Serializable]
public class CharacterData
{
    public int level         = 1;
    public int currentAP     = 10;
    public int maxAP         = 10;
    public int totalConsumed = 0;
    public int day           = 1;
    public int currentHP     = 100;
    public int maxHP         = 100;
    public int currentMP     = 50;
    public int maxMP         = 50;

    // 향후 확장 예시:
    // public float posX, posY, posZ;
    // public List<string> inventory = new();
}
