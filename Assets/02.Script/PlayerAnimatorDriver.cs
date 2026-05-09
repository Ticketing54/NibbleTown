using UnityEngine;

// S: 애니메이터 구동만 담당
// D: 구체 클래스 PlayerController가 아닌 IPlayerState에 의존
[RequireComponent(typeof(Animator))]
public class PlayerAnimatorDriver : MonoBehaviour
{
    private IPlayerState _state;
    private Animator _animator;

    private static readonly int SpeedHash      = Animator.StringToHash("Speed");
    private static readonly int JumpHash       = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _state    = GetComponent<IPlayerState>();
    }

    private void OnEnable()  => _state.OnJumped += TriggerJump;
    private void OnDisable() => _state.OnJumped -= TriggerJump;

    private void Update()
    {
        bool hasMoveInput  = _state.MoveInputMagnitude > 0.01f;
        float targetSpeed  = hasMoveInput ? (_state.IsSprinting ? 1f : 0.5f) : 0f;
        float currentSpeed = _animator.GetFloat(SpeedHash);

        _animator.SetFloat(SpeedHash, Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f));
        _animator.SetBool(IsGroundedHash, _state.IsGrounded);
    }

    private void TriggerJump() => _animator.SetTrigger(JumpHash);
}
