public interface IInteractable
{
    string HintText { get; }
    int    APCost   { get; }
    void OnStartInteract(IMovementLock _mover);
    void OnCompleteInteract(IMovementLock _mover);
    void OnCancelInteract(IMovementLock _mover);
}
