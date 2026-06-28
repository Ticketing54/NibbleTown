using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStat))]
public class PlayerAttack : MonoBehaviour, IWeaponState
{
    [SerializeField] private HitBox              hitBox;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private LayerMask           monsterLayer;
    [SerializeField] private float               autoAimRadius = 2f;

    public LayerMask MonsterLayer => monsterLayer;
    public event System.Action OnAttackTriggered;

    private CharacterStatConfig config;
    private IMovementLock       movementLock;
    private float               cooldownTimer;
    private bool                isCombatMode;
    private float               attackSpeedMult = 1f;

    public void SetAttackSpeedMultiplier(float mult) => attackSpeedMult = Mathf.Max(0.01f, mult);

    public bool IsAttacking { get; set; }

    private void OnEnable()
    {
        attackAction.action.Enable();
        attackAction.action.performed += OnAttackInput;
        GameEvents.OnNightBegin += EnterCombatMode;
        GameEvents.OnDayBegin   += ExitCombatMode;
        movementLock?.LockMovement(false);
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttackInput;
        attackAction.action.Disable();
        GameEvents.OnNightBegin -= EnterCombatMode;
        GameEvents.OnDayBegin   -= ExitCombatMode;
    }

    private void OnAttackInput(InputAction.CallbackContext _ctx) => TryAttack();

    public void Init(CharacterStatConfig _config)
    {
        config       = _config;
        movementLock = GetComponent<IMovementLock>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public bool TryAttack()
    {
        if (!isCombatMode)      return false;
        if (config == null)     return false;
        if (IsAttacking)        return false;
        if (cooldownTimer > 0f) return false;

        cooldownTimer = config.attackCooldown / attackSpeedMult;

        Transform nearestMonster = FindNearestTarget();
        if (nearestMonster != null)
            movementLock?.LookAt(nearestMonster);

        movementLock?.LockMovement(true);
        OnAttackTriggered?.Invoke();
        return true;
    }

    public void UnlockMovement()
    {
        movementLock?.LookAt(null);
        movementLock?.LockMovement(false);
    }

    private Transform FindNearestTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, autoAimRadius, monsterLayer);
        if (cols.Length == 0) return null;

        Transform nearest = null;
        float minSqrDist = float.MaxValue;
        foreach (Collider col in cols)
        {
            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest    = col.transform;
            }
        }
        return nearest;
    }

    // 애니메이션 이벤트에서 호출 — 타격 프레임
    public void ActivateHitBox()
    {
        if (hitBox == null) return;
        if (config == null) return;
        hitBox.Activate(config.attackDamage, gameObject, config.critChance, config.critBonusRate);
    }

    private void EnterCombatMode() => isCombatMode = true;

    private void ExitCombatMode()
    {
        isCombatMode = false;
        movementLock?.LockMovement(false);
    }
}
