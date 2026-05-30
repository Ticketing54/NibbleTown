using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MonsterController))]
public class MonsterAnimatorDriver : MonoBehaviour
{
    [SerializeField] private float damageAnimDuration  = 0.4f;
    [SerializeField] private bool  hasDamageAnim       = true;
    [SerializeField] private float hitFrameNormalized  = 0.4f;

    private static readonly int AnimHash = Animator.StringToHash("animation");
    private const int AnimIdle   = 1;
    private const int AnimMove   = 2;
    private const int AnimAttack = 3;
    private const int AnimDamage = 4;
    private const int AnimDie    = 5;

    private Animator          animator;
    private MonsterController controller;
    private float             damageAnimTimer;
    private bool              hitFired;

    private void Awake()
    {
        animator   = GetComponent<Animator>();
        controller = GetComponent<MonsterController>();
    }

    private void OnEnable()
    {
        controller.OnStateChanged += HandleStateChanged;
        controller.OnHit          += HandleHit;
        controller.OnDied         += HandleDied;
    }

    private void OnDisable()
    {
        controller.OnStateChanged -= HandleStateChanged;
        controller.OnHit          -= HandleHit;
        controller.OnDied         -= HandleDied;
    }

    private void Update()
    {
        if (damageAnimTimer > 0f)
        {
            damageAnimTimer -= Time.deltaTime;
            if (damageAnimTimer <= 0f)
                ApplyAnimForState(controller.CurrentState);
        }

        UpdateAttackHit();
    }

    private void UpdateAttackHit()
    {
        bool inAttack = controller.CurrentState == MonsterController.State.AttackBuilding ||
                        controller.CurrentState == MonsterController.State.AttackPlayer;

        if (!inAttack) { hitFired = false; return; }

        float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        if (!hitFired && t >= hitFrameNormalized)
        {
            hitFired = true;
            controller.LaunchAttack();
        }
        else if (t < hitFrameNormalized)
        {
            hitFired = false;
        }
    }

    private void HandleStateChanged(MonsterController.State state)
    {
        // 피격 애니메이션 재생 중엔 상태 전환 애니메이션 무시
        if (damageAnimTimer > 0f) return;
        ApplyAnimForState(state);
    }

    private void HandleHit()
    {
        if (!hasDamageAnim) return;
        animator.SetInteger(AnimHash, AnimDamage);
        damageAnimTimer = damageAnimDuration;
    }

    private void HandleDied()
    {
        damageAnimTimer = 0f;
        animator.SetInteger(AnimHash, AnimDie);
    }

    private void ApplyAnimForState(MonsterController.State state)
    {
        int anim = state switch
        {
            MonsterController.State.MoveToBuilding => AnimMove,
            MonsterController.State.ChasePlayer    => AnimMove,
            MonsterController.State.AttackBuilding => AnimAttack,
            MonsterController.State.AttackPlayer   => AnimAttack,
            MonsterController.State.Dead           => AnimDie,
            _                                      => AnimIdle,
        };
        animator.SetInteger(AnimHash, anim);
    }
}
