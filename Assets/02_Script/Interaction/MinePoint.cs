using UnityEngine;

public class MinePoint : InteractableBase
{
    [SerializeField] private DropTable dropTable;

    public override string HintText => "[F] 채광하기";

    private void GiveDrops() => dropTable?.GiveTo(cachedInventory);

    private void Reset() { apCost = 2; holdDuration = 2f; animType = InteractionAnimType.Mine; }

    public override void OnStartInteract(IMovementLock _mover)    { }
    public override void OnCancelInteract(IMovementLock _mover)   { }

    public override void OnCompleteInteract(IMovementLock _mover) => GiveDrops();
}
