using UnityEngine;

public class FishingSpot : InteractableBase
{
    [SerializeField] private int itemId    = 1012; // 피라미
    [SerializeField] private int itemCount = 1;

    public override string HintText => "[F] 낚시하기";

    private void Reset() { apCost = 2; holdDuration = 3f; animType = InteractionAnimType.Fish; }

    public override void OnStartInteract(IMovementLock _mover)    { }
    public override void OnCancelInteract(IMovementLock _mover)   { }

    public override void OnCompleteInteract(IMovementLock _mover)
    {
        cachedInventory?.AddItem(itemId, itemCount);
    }
}
