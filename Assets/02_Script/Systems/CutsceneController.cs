using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private InputActionReference skipAction;
    [SerializeField] private PlayableDirector     dayNightDirector;

    public event Action OnFinished;

    private Coroutine dayNightCoroutine;
    private bool      skipped;

    // ── 밤낮 전환 ─────────────────────────────────────────────────────────────

    public void PlayToNight() => StartDayNightSegment(0.0, 3.0);
    public void PlayToDay()   => StartDayNightSegment(3.0, 6.0);

    private void StartDayNightSegment(double _startTime, double _endTime)
    {
        if (dayNightCoroutine != null) StopCoroutine(dayNightCoroutine);
        skipped = false;
        dayNightCoroutine = StartCoroutine(CoDayNightSegment(_startTime, _endTime));
        EnableSkip();
    }

    private IEnumerator CoDayNightSegment(double _startTime, double _endTime)
    {
        dayNightDirector.extrapolationMode = DirectorWrapMode.Hold;
        dayNightDirector.time = _startTime;
        dayNightDirector.Play();

        while (!skipped && dayNightDirector.time < _endTime)
            yield return null;

        if (skipped)
        {
            dayNightDirector.time = _endTime;
            dayNightDirector.Evaluate();
        }

        dayNightDirector.Pause();
        dayNightCoroutine = null;

        DisableSkip();
        OnFinished?.Invoke();
    }

    // ── Skip ──────────────────────────────────────────────────────────────────

    public void Skip()
    {
        if (skipped) return;
        skipped = true;
    }

    private void EnableSkip()
    {
        if (skipAction == null) return;
        skipAction.action.Enable();
        skipAction.action.performed += OnSkipInput;
    }

    private void DisableSkip()
    {
        if (skipAction == null) return;
        skipAction.action.performed -= OnSkipInput;
    }

    private void OnSkipInput(InputAction.CallbackContext _ctx) => Skip();
}
