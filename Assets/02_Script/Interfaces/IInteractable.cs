using UnityEngine;

public enum InteractionAnimType { None = 0, Gather = 1, Fish = 2, Mine = 3, Build = 4 }

public interface IInteractable
{
    string              HintText     { get; }
    int                 APCost       { get; }
    float               HoldDuration { get; }
    InteractionAnimType AnimType     { get; }
    Transform           Transform    { get; }
    void OnStartInteract(IMovementLock _mover);
    void OnCompleteInteract(IMovementLock _mover);
    void OnCancelInteract(IMovementLock _mover);
}
