using UnityEngine;

public class GatherPoint : InteractableBase
{
    [SerializeField] private DropTable dropTable;

    public override string HintText => "[F] 채집하기";

    private void GiveDrops() => dropTable?.GiveTo(cachedInventory);

    private void Reset() { apCost = 1; holdDuration = 1.5f; animType = InteractionAnimType.Gather; }

    public override void OnStartInteract(IMovementLock _mover)    { }
    public override void OnCancelInteract(IMovementLock _mover)   { }

    public override void OnCompleteInteract(IMovementLock _mover) => GiveDrops();
}
