using UnityEngine;

public class CharacterStat : MonoBehaviour, IActionPoints, IDamageable
{
    [SerializeField] private string            characterId    = "Barbarian";
    [SerializeField] private CharacterStatConfig defaultConfig;

    private CharacterInitData initData;
    private CharacterData     runtimeData = new CharacterData();
    private bool              lowFired;

    public string              Id     => initData.id;
    public CharacterStatConfig Config => defaultConfig;

    // HP (IDamageable)
    public bool IsDead   => runtimeData.currentHP <= 0;
    public int CurrentHP => runtimeData.currentHP;
    public int MaxHP     => runtimeData.maxHP;

    // MP
    public int CurrentMP => runtimeData.currentMP;
    public int MaxMP     => runtimeData.maxMP;

    // AP (IActionPoints)
    public int Current => runtimeData.currentAP;
    public int Max     => runtimeData.maxAP;

    // Day
    public int Day => runtimeData.day;


    private void Awake()
    {
        Init(BuildInitData());
        GetComponent<PlayerAttack>()?.Init(defaultConfig);
    }

    private void OnEnable()
    {
        GameEvents.OnDayAdvanced += AdvanceDay;
    }

    private void OnDisable()
    {
        GameEvents.OnDayAdvanced -= AdvanceDay;
    }

    public void Init(CharacterInitData _data)
    {
        initData = _data;

        runtimeData.maxHP     = _data.maxHP;
        runtimeData.currentHP = _data.maxHP;
        runtimeData.maxMP     = _data.maxMP;
        runtimeData.currentMP = _data.maxMP;
        runtimeData.maxAP     = _data.baseMaxAP;
        runtimeData.currentAP = _data.baseMaxAP;
        lowFired = false;
    }

    // ── HP ───────────────────────────────────────────────────

    public void TakeDamage(int _amount)
    {
        runtimeData.currentHP = Mathf.Max(0, runtimeData.currentHP - _amount);
        GameEvents.RaiseHPChanged(runtimeData.currentHP, runtimeData.maxHP);
    }

    public void Heal(int _amount)
    {
        runtimeData.currentHP = Mathf.Min(runtimeData.maxHP, runtimeData.currentHP + _amount);
        GameEvents.RaiseHPChanged(runtimeData.currentHP, runtimeData.maxHP);
    }

    // ── MP ───────────────────────────────────────────────────

    public void UseMP(int _amount)
    {
        runtimeData.currentMP = Mathf.Max(0, runtimeData.currentMP - _amount);
        GameEvents.RaiseMPChanged(runtimeData.currentMP, runtimeData.maxMP);
    }

    public void RestoreMP(int _amount)
    {
        runtimeData.currentMP = Mathf.Min(runtimeData.maxMP, runtimeData.currentMP + _amount);
        GameEvents.RaiseMPChanged(runtimeData.currentMP, runtimeData.maxMP);
    }

    // ── AP (IActionPoints) ───────────────────────────────────

    public bool CanSpend(int _amount) => runtimeData.currentAP >= _amount;

    public bool TrySpend(int _amount)
    {
        if (!CanSpend(_amount)) return false;

        runtimeData.currentAP -= _amount;

        CheckLow();
        GameEvents.RaiseAPChanged(runtimeData.currentAP, runtimeData.maxAP);
        return true;
    }

    public void Restore()
    {
        runtimeData.currentAP = runtimeData.maxAP;
        lowFired = false;
        GameEvents.RaiseAPChanged(runtimeData.currentAP, runtimeData.maxAP);
    }

    // ── Day ──────────────────────────────────────────────────

    public void AdvanceDay()
    {
        runtimeData.day++;
        Restore();
        GameEvents.RaiseDayChanged(runtimeData.day);
    }

    // ── 저장/불러오기 ─────────────────────────────────────────

    public CharacterData     GetSaveData() => runtimeData;
    public CharacterInitData GetInitData() => initData;

    public void LoadSaveData(CharacterData _data)
    {
        runtimeData = _data;
        lowFired    = false;
    }

    // ── 내부 ─────────────────────────────────────────────────

    private CharacterInitData BuildInitData()
    {
        if (defaultConfig != null)
        {
            return new CharacterInitData
            {
                id           = characterId,
                maxHP        = defaultConfig.baseMaxHP,
                maxMP        = defaultConfig.baseMaxMP,
                baseMaxAP    = defaultConfig.baseMaxAP,
                lowThreshold = defaultConfig.lowThreshold,
            };
        }

        var fallback = CharacterInitData.Default;
        fallback.id  = characterId;
        return fallback;
    }

    private void CheckLow()
    {
        if (lowFired) return;
        if ((float)runtimeData.currentAP / runtimeData.maxAP < initData.lowThreshold)
        {
            lowFired = true;
            GameEvents.RaiseAPNotEnough();
        }
    }
}
