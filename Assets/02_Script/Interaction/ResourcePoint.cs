using UnityEngine;

public class ResourcePoint : InteractableBase
{
    [SerializeField] private DropTable dropTable;
    [SerializeField] private string    hintText = "[F] 상호작용";

    public override string HintText => hintText;

    public override bool CanInteract() => cachedInventory == null || !cachedInventory.IsFull;

    public override void OnStartInteract(IMovementLock _mover)    { }
    public override void OnCancelInteract(IMovementLock _mover)   { }

    public override void OnCompleteInteract(IMovementLock _mover) => dropTable?.GiveTo(cachedInventory);
}
