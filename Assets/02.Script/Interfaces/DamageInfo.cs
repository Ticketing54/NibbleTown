using UnityEngine;

public struct DamageInfo
{
    public GameObject dealer;
    public int        amount;
    public bool       isCrit;

    public DamageInfo(GameObject _dealer, int _amount, bool _isCrit = false)
    {
        dealer = _dealer;
        amount = _amount;
        isCrit = _isCrit;
    }
}
