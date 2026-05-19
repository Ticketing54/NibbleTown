using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] private CutsceneController cutscene;
    [SerializeField] private CinemachineCamera  wholeExteriorCam;

    private void Awake()
    {
        SetVCamPriority(wholeExteriorCam, 20);
    }

    private void OnEnable()
    {
        GameEvents.OnNightRequested   += OnNightRequested;
        GameEvents.OnNextDayRequested += OnNextDayRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnNightRequested   -= OnNightRequested;
        GameEvents.OnNextDayRequested -= OnNextDayRequested;
    }

    private void Start()
    {
        SetPlayerEnabled(false);
        StartCoroutine(CoIntro());
    }

    private IEnumerator CoIntro()
    {
        yield return new WaitForSeconds(3.5f);
        SetVCamPriority(wholeExteriorCam, 0);
        yield return new WaitForSeconds(2f);
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

    private void OnNightRequested()
    {
        cutscene.OnFinished += OnNightFinished;
        cutscene.PlayToNight();

        void OnNightFinished()
        {
            cutscene.OnFinished -= OnNightFinished;
            GameEvents.RaiseNightBegin();
        }
    }

    private void OnNextDayRequested()
    {
        cutscene.OnFinished += OnDayFinished;
        cutscene.PlayToDay();

        void OnDayFinished()
        {
            cutscene.OnFinished -= OnDayFinished;
            GameEvents.RaiseDayBegin();
        }
    }

    private void SetPlayerEnabled(bool _enabled)
    {
        Debug.Log($"[PSC] SetPlayerEnabled({_enabled})");
        GameEvents.RaisePlayerInputLocked(!_enabled);
    }
}
