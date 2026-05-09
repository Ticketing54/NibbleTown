public interface IInteractable
{
    string HintText { get; }
    int    APCost   { get; }
    void OnStartInteract(IMovementLock mover);
    void OnCompleteInteract(IMovementLock mover);
    void OnCancelInteract(IMovementLock mover);
}
