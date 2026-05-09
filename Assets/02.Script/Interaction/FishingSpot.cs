using UnityEngine;

public class FishingSpot : InteractableBase
{
    public override string HintText => "[F] 낚시하기";

    private void Reset() => apCost = 2;

    public override void OnStartInteract(IMovementLock mover)
    {
        Debug.Log("[낚시] 시작");
    }

    public override void OnCompleteInteract(IMovementLock mover)
    {
        Debug.Log("[낚시] 완료 → 아이템 획득");
    }

    public override void OnCancelInteract(IMovementLock mover)
    {
        Debug.Log("[낚시] 취소");
    }
}
