using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected int                 apCost       = 1;
    [SerializeField] protected float              holdDuration = 1.5f;
    [SerializeField] protected InteractionAnimType animType    = InteractionAnimType.None;

    public abstract string              HintText     { get; }
    public virtual  int                 APCost       => apCost;
    public virtual  float               HoldDuration => holdDuration;
    public virtual  InteractionAnimType AnimType     => animType;
    public          Transform           Transform    => transform;

    protected Inventory cachedInventory;

    protected virtual void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.TryGetComponent<Inventory>(out var inv))
            cachedInventory = inv;
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.TryGetComponent<Inventory>(out _))
            cachedInventory = null;
    }

    public virtual  bool CanInteract()                          => true;
    public abstract void OnStartInteract(IMovementLock _mover);
    public abstract void OnCompleteInteract(IMovementLock _mover);
    public abstract void OnCancelInteract(IMovementLock _mover);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
