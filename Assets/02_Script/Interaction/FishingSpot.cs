using UnityEngine;

public class FishingSpot : InteractableBase
{
    [SerializeField] private DropTable dropTable;

    public override string HintText => "[F] 낚시하기";

    private void GiveDrops() => dropTable?.GiveTo(cachedInventory);

    private void Reset() { apCost = 2; holdDuration = 3f; animType = InteractionAnimType.Fish; }

    public override void OnStartInteract(IMovementLock _mover)    { }
    public override void OnCancelInteract(IMovementLock _mover)   { }

    public override void OnCompleteInteract(IMovementLock _mover) => GiveDrops();
}
