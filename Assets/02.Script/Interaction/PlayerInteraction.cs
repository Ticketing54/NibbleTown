using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float detectRadius = 2f;
    [SerializeField] private float holdDuration = 1.5f;

    [Tooltip("IInteractionUI를 구현한 MonoBehaviour (UIManager)")]
    [SerializeField] private MonoBehaviour interactionUISource;

    [Tooltip("IProgressBar를 구현한 MonoBehaviour (HoldProgressBar)")]
    [SerializeField] private MonoBehaviour progressBarSource;

    [Tooltip("IActionPoints를 구현한 MonoBehaviour (ActionPointSystem)")]
    [SerializeField] private MonoBehaviour actionPointsSource;

    private IMovementLock  _mover;
    private IInteractionUI _ui;
    private IProgressBar   _bar;
    private IActionPoints  _ap;

    private IInteractable _nearest;
    private IInteractable _active;
    private bool  _holding;
    private float _holdStartTime;

    private void Awake()
    {
        _mover = GetComponent<IMovementLock>();
        _ui    = interactionUISource as IInteractionUI;
        _bar   = progressBarSource  as IProgressBar;
        _ap    = actionPointsSource as IActionPoints;

        if (_ui == null)
            Debug.LogError($"[PlayerInteraction] {interactionUISource} does not implement IInteractionUI");
        if (_bar == null)
            Debug.LogError($"[PlayerInteraction] {progressBarSource} does not implement IProgressBar");
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

    private void Update()
    {
        RefreshNearest();

        if (!_holding || _active == null) return;

        float elapsed  = Time.time - _holdStartTime;
        float progress = elapsed / holdDuration;
        _bar.SetProgress(progress);

        if (elapsed >= holdDuration)
        {
            _active.OnCompleteInteract(_mover);
            FinishInteraction(completed: true);
        }
    }

    private void RefreshNearest()
    {
        var hits = Physics.OverlapSphere(transform.position, detectRadius);
        IInteractable best = null;
        float bestDist = float.MaxValue;

        foreach (var col in hits)
        {
            var it = col.GetComponentInParent<IInteractable>();
            if (it == null) continue;
            float d = Vector3.Distance(transform.position, col.transform.position);
            if (d < bestDist) { bestDist = d; best = it; }
        }

        if (best == _nearest) return;

        _nearest = best;
        if (_nearest != null)
        {
            bool canAfford = _ap == null || _ap.CanSpend(_nearest.APCost);
            string hint = canAfford
                ? $"{_nearest.HintText}  [{_nearest.APCost}AP]"
                : $"[행동력 부족]  ({_nearest.APCost}AP 필요)";
            _ui.ShowHint(hint);
        }
        else
        {
            _ui.HideHint();
        }
    }

    private void OnStarted(InputAction.CallbackContext ctx)
    {
        if (_nearest == null || _holding) return;

        if (_ap != null && !_ap.CanSpend(_nearest.APCost))
        {
            Debug.Log($"[PlayerInteraction] 행동력 부족 — {_nearest.APCost}AP 필요, 현재 {_ap.Current}AP");
            return;
        }

        _active        = _nearest;
        _holding       = true;
        _holdStartTime = Time.time;
        _active.OnStartInteract(_mover);
        _mover.LockMovement(true);
        _bar.Show(true);
    }

    private void OnCanceled(InputAction.CallbackContext ctx)
    {
        if (!_holding) return;
        _active.OnCancelInteract(_mover);
        FinishInteraction(completed: false);
    }

    private void FinishInteraction(bool completed)
    {
        if (completed && _ap != null)
            _ap.TrySpend(_active.APCost);

        _holding = false;
        _mover.LockMovement(false);
        _bar.Show(false);
        _active = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
