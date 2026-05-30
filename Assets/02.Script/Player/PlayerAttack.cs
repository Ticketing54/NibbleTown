using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStat))]
public class PlayerAttack : MonoBehaviour, IWeaponState
{
    [SerializeField] private HitBox              hitBox;
    [SerializeField] private InputActionReference attackAction;

    public event System.Action OnAttackTriggered;

    private CharacterStatConfig config;
    private IMovementLock       movementLock;
    private float               cooldownTimer;
    private bool                isCombatMode;

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

        cooldownTimer = config.attackCooldown;
        movementLock?.LockMovement(true);
        OnAttackTriggered?.Invoke();
        return true;
    }

    public void UnlockMovement() => movementLock?.LockMovement(false);

    // 애니메이션 이벤트에서 호출 — 타격 프레임
    public void ActivateHitBox()
    {
        if (hitBox == null) return;
        hitBox.Activate(config != null ? config.attackDamage : 0, gameObject);
    }

    private void EnterCombatMode() => isCombatMode = true;

    private void ExitCombatMode()
    {
        isCombatMode = false;
        movementLock?.LockMovement(false);
    }
}
