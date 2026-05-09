using UnityEngine;

// S: 애니메이터 구동만 담당
// D: 구체 클래스 PlayerController가 아닌 IPlayerState에 의존
[RequireComponent(typeof(Animator))]
public class PlayerAnimatorDriver : MonoBehaviour
{
    private IPlayerState state;
    private Animator animator;

    private static readonly int SpeedHash      = Animator.StringToHash("Speed");
    private static readonly int JumpHash       = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        state    = GetComponent<IPlayerState>();
    }

    private void OnEnable()  => state.OnJumped += TriggerJump;
    private void OnDisable() => state.OnJumped -= TriggerJump;

    private void Update()
    {
        bool hasMoveInput  = state.MoveInputMagnitude > 0.01f;
        float targetSpeed  = hasMoveInput ? (state.IsSprinting ? 1f : 0.5f) : 0f;
        float currentSpeed = animator.GetFloat(SpeedHash);

        animator.SetFloat(SpeedHash, Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f));
        animator.SetBool(IsGroundedHash, state.IsGrounded);
    }

    private void TriggerJump() => animator.SetTrigger(JumpHash);
}
