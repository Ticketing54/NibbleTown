using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    private PlayerAttack playerAttack;

    public override void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        if (playerAttack == null)
            playerAttack = _animator.GetComponent<PlayerAttack>();

        _animator.applyRootMotion = false;
        if (playerAttack != null) playerAttack.IsAttacking = true;
    }

    public override void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        _animator.applyRootMotion = false;
        if (playerAttack != null) playerAttack.IsAttacking = false;
        playerAttack?.UnlockMovement();
    }
}
