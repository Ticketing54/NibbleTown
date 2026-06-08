using System;

public interface IPlayerState
{
    float MoveInputMagnitude { get; }
    bool IsGrounded { get; }
    bool IsSprinting { get; }
    event Action OnJumped;
}
