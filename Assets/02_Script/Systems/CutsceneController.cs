using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private DayNightController dayNightController;

    [Header("Intro")]
    [SerializeField] private CinemachineCamera introCam;
    [SerializeField] private float camSwitchDelay = 3.5f;
    [SerializeField] private float introTailDelay  = 2f;

    private void Awake()
    {
        SetVCamPriority(introCam, 20);
    }

    private void OnEnable()
    {
        GameEvents.OnNightRequested   += HandleNightRequested;
        GameEvents.OnNextDayRequested += HandleNextDayRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnNightRequested   -= HandleNightRequested;
        GameEvents.OnNextDayRequested -= HandleNextDayRequested;
    }

    private void Start()
    {
        SetPlayerEnabled(false);
        StartCoroutine(CoIntro());
    }

    private IEnumerator CoIntro()
    {
        yield return new WaitForSeconds(camSwitchDelay);
        SetVCamPriority(introCam, 0);
        yield return new WaitForSeconds(introTailDelay);

        SetPlayerEnabled(true);
        GameEvents.RaiseDayBegin();
    }

    private void SetVCamPriority(CinemachineCamera _cam, int _value)
    {
        var p     = _cam.Priority;
        p.Enabled = true;
        p.Value   = _value;
        _cam.Priority = p;
    }

    private void SetPlayerEnabled(bool _enabled) => GameEvents.RaisePlayerInputLocked(!_enabled);

    // ── 밤낮 전환 ─────────────────────────────────────────────────────────────

    private void HandleNightRequested()   => RequestDayNightChange(true);
    private void HandleNextDayRequested() => RequestDayNightChange(false);

    private void RequestDayNightChange(bool _toNight)
    {
        if (dayNightController == null)
        {
            RaiseDayNightBegin(_toNight);
            return;
        }

        dayNightController.OnFinished += OnDayNightFinished;
        if (_toNight) dayNightController.PlayToNight();
        else          dayNightController.PlayToDay();

        void OnDayNightFinished()
        {
            dayNightController.OnFinished -= OnDayNightFinished;
            RaiseDayNightBegin(_toNight);
        }
    }

    private void RaiseDayNightBegin(bool _toNight)
    {
        if (_toNight) GameEvents.RaiseNightBegin();
        else          GameEvents.RaiseDayBegin();
    }
}
