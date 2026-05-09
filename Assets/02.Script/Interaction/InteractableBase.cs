using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected int apCost = 1;

    public abstract string HintText { get; }
    public virtual  int    APCost   => apCost;

    protected virtual void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public abstract void OnStartInteract(IMovementLock mover);
    public abstract void OnCompleteInteract(IMovementLock mover);
    public abstract void OnCancelInteract(IMovementLock mover);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
