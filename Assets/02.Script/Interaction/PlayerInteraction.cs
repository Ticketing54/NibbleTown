using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private MonoBehaviour actionPointsSource;

    private IInteractable nearInteractable;
    private Coroutine interactionRoutine;
    private IMovementLock  mover;
    private IActionPoints  ap;

    private bool  holding;
    private float holdStartTime;

    private void Awake()
    {
        mover = GetComponent<IMovementLock>();
        ap    = actionPointsSource as IActionPoints;
    }

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.started  += OnStarted;
        interactAction.action.canceled += OnCanceled;
    }

    private void OnDisable()
    {
        interactAction.action.started  -= OnStarted;
        interactAction.action.canceled -= OnCanceled;
        interactAction.action.Disable();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.TryGetComponent<IInteractable>(out IInteractable newInteractable))
        {
            nearInteractable = newInteractable;

            bool canAfford = ap == null || ap.CanSpend(nearInteractable.APCost);
            string hint = canAfford
                ? $"{nearInteractable.HintText}  [{nearInteractable.APCost}AP]"
                : $"[행동력 부족]  ({nearInteractable.APCost}AP 필요)";
                GameEvents.RaiseInteractionTextShow(true,hint);
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        GameEvents.RaiseInteractionTextShow(false,string.Empty);
    }

    private void OnStarted(InputAction.CallbackContext _ctx)
    {
        if (nearInteractable == null || holding) return;

        if (ap != null && !ap.CanSpend(nearInteractable.APCost))
        {
            Debug.Log($"[PlayerInteraction] 행동력 부족 — {nearInteractable.APCost}AP 필요, 현재 {ap.Current}AP");
            return;
        }

        holding       = true;
        holdStartTime = Time.time;
        nearInteractable.OnStartInteract(mover);
        mover.LockMovement(true);
        GameEvents.RaiseProgressBarShow(true);
        interactionRoutine = StartCoroutine(CoStartInteraction());
    }

    private void OnCanceled(InputAction.CallbackContext _ctx)
    {
        if (!holding) return;
        if (interactionRoutine != null)
            StopCoroutine(interactionRoutine);
        nearInteractable.OnCancelInteract(mover);
        FinishInteraction(false);
    }

    private void FinishInteraction(bool _completed)
    {
        interactionRoutine = null;
        if (_completed && ap != null)
            ap.TrySpend(nearInteractable.APCost);

        holding = false;
        mover.LockMovement(false);
        GameEvents.RaiseProgressBarShow(false);
    }

    private IEnumerator CoStartInteraction()
    {
        while (true)
        {
            float elapsed  = Time.time - holdStartTime;
            float progress = elapsed / holdDuration;
            GameEvents.RaiseProgressBarSetProgress(progress);

            if (progress >= 1f)
            {
                nearInteractable.OnCompleteInteract(mover);
                FinishInteraction(true);
                yield break;
            }

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
    }
}
