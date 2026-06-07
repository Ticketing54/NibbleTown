using UnityEngine;

public class BuildingRepairPoint : InteractableBase
{
    [SerializeField]private BuildingHealth buildingHealth;

    public override string HintText => buildingHealth != null && buildingHealth.CanRepair
        ? "건물 수리"
        : "수리 불필요";

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnStartInteract(IMovementLock _mover)
    {
        if (buildingHealth != null && !buildingHealth.CanRepair)
        {
            _mover.LockMovement(false);
            GameEvents.RaiseInteractionTextShow(true, "수리할 필요가 없습니다");
        }
    }

    public override void OnCompleteInteract(IMovementLock _mover)
    {
        if (buildingHealth == null || !buildingHealth.CanRepair) return;
        buildingHealth.Repair();
    }

    public override void OnCancelInteract(IMovementLock _mover) { }
}
