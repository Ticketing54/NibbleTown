using UnityEngine;

public class BuildSite : InteractableBase
{
    public override string HintText => "[F] 건설하기";

    private void Reset() => apCost = 3;

    public override void OnStartInteract(IMovementLock _mover)    => Debug.Log("[건설] 시작");
    public override void OnCompleteInteract(IMovementLock _mover) => Debug.Log("[건설] 완료");
    public override void OnCancelInteract(IMovementLock _mover)   => Debug.Log("[건설] 취소");
}
