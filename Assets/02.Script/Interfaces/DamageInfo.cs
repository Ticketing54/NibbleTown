using UnityEngine;

public struct DamageInfo
{
    public GameObject dealer;
    public int        amount;

    public DamageInfo(GameObject _dealer, int _amount)
    {
        dealer = _dealer;
        amount = _amount;
    }
}
