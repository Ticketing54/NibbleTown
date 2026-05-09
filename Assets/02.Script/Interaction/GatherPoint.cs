using UnityEngine;

public class GatherPoint : InteractableBase
{
    public override string HintText => "[F] 채집하기";

    private void Reset() => apCost = 1;

    public override void OnStartInteract(IMovementLock _mover)    => Debug.Log("[채집] 시작");
    public override void OnCompleteInteract(IMovementLock _mover) => Debug.Log("[채집] 완료");
    public override void OnCancelInteract(IMovementLock _mover)   => Debug.Log("[채집] 취소");
}
