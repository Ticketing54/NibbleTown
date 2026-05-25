using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorDriver : MonoBehaviour
{
    private IPlayerState      state;
    private IWeaponState      weaponState;
    private IInteractionState interactionState;
    private Animator          animator;

    private static readonly int SpeedHash           = Animator.StringToHash("Speed");
    private static readonly int JumpHash            = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash      = Animator.StringToHash("IsGrounded");
    private static readonly int AttackHash          = Animator.StringToHash("Attack");
    private static readonly int InteractingHash     = Animator.StringToHash("Interacting");
    private static readonly int InteractionAnimHash = Animator.StringToHash("InteractionAnim");
    private static readonly int DenyAPHash          = Animator.StringToHash("DenyAP");
    private static readonly int IsCombatModeHash    = Animator.StringToHash("IsCombatMode");
    private static readonly int WeaponTypeHash      = Animator.StringToHash("WeaponType");

    private void Awake()
    {
        animator         = GetComponent<Animator>();
        state            = GetComponent<IPlayerState>();
        weaponState      = GetComponent<IWeaponState>();
        interactionState = GetComponent<IInteractionState>();

        CharacterStat stat = GetComponent<CharacterStat>();
        if (stat != null && stat.Config != null)
            animator.SetFloat(WeaponTypeHash, (float)stat.Config.weaponType);
    }

    private void OnEnable()
    {
        state.OnJumped += TriggerJump;
        if (weaponState != null)
            weaponState.OnAttackTriggered += TriggerAttack;
        if (interactionState != null)
        {
            interactionState.OnInteractionStarted += OnInteractionStarted;
            interactionState.OnInteractionEnded   += OnInteractionEnded;
            interactionState.OnInteractionDenied  += OnInteractionDenied;
        }
        GameEvents.OnNightBegin += EnterCombatMode;
        GameEvents.OnDayBegin   += ExitCombatMode;
    }

    private void OnDisable()
    {
        state.OnJumped -= TriggerJump;
        if (weaponState != null)
            weaponState.OnAttackTriggered -= TriggerAttack;
        if (interactionState != null)
        {
            interactionState.OnInteractionStarted -= OnInteractionStarted;
            interactionState.OnInteractionEnded   -= OnInteractionEnded;
            interactionState.OnInteractionDenied  -= OnInteractionDenied;
        }
        GameEvents.OnNightBegin -= EnterCombatMode;
        GameEvents.OnDayBegin   -= ExitCombatMode;
    }

    private void Update()
    {
        bool hasMoveInput  = state.MoveInputMagnitude > 0.01f;
        float targetSpeed  = hasMoveInput ? (state.IsSprinting ? 1f : 0.5f) : 0f;
        float currentSpeed = animator.GetFloat(SpeedHash);

        animator.SetFloat(SpeedHash, Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f));
        animator.SetBool(IsGroundedHash, state.IsGrounded);
    }

    private void TriggerJump()   => animator.SetTrigger(JumpHash);
    private void TriggerAttack() => animator.SetTrigger(AttackHash);

    private void OnInteractionStarted(InteractionAnimType _animType)
    {
        animator.SetBool(InteractingHash, true);
        animator.SetInteger(InteractionAnimHash, (int)_animType);
    }

    private void OnInteractionEnded()
    {
        animator.SetBool(InteractingHash, false);
        animator.SetInteger(InteractionAnimHash, (int)InteractionAnimType.None);
    }

    private void OnInteractionDenied() => animator.SetTrigger(DenyAPHash);

    private void EnterCombatMode() => animator.SetBool(IsCombatModeHash, true);
    private void ExitCombatMode()  => animator.SetBool(IsCombatModeHash, false);
}
