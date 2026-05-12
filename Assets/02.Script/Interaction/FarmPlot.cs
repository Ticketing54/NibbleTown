using UnityEngine;

public class FarmPlot : InteractableBase
{
    public override string HintText => "[F] 농사짓기";

    private void Reset() { apCost = 2; holdDuration = 2f; animType = InteractionAnimType.Farm; }

    public override void OnStartInteract(IMovementLock _mover)    => Debug.Log("[농사] 시작");
    public override void OnCompleteInteract(IMovementLock _mover) => Debug.Log("[농사] 완료");
    public override void OnCancelInteract(IMovementLock _mover)   => Debug.Log("[농사] 취소");
}
