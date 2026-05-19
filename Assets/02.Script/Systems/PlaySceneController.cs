using System;
using UnityEngine;
using UnityEngine.Playables;

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] private CutsceneController cutscene;

    [Header("Cutscenes")]
    [SerializeField] private PlayableDirector introCutscene;
    [SerializeField] private PlayableDirector dayToNightCutscene;
    [SerializeField] private PlayableDirector nightToDayCutscene;

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
        PlayCutscene(introCutscene, () =>
        {
            SetPlayerEnabled(true);
            GameEvents.RaiseDayBegin();
        });
    }

    private void OnNightRequested()
    {
        PlayCutscene(dayToNightCutscene, () =>
        {
            SetPlayerEnabled(true);
            GameEvents.RaiseNightBegin();
        });
    }

    private void OnNextDayRequested()
    {
        PlayCutscene(nightToDayCutscene, () =>
        {
            SetPlayerEnabled(true);
            GameEvents.RaiseDayBegin();
        });
    }

    private void PlayCutscene(PlayableDirector _director, Action _onFinished)
    {
        SetPlayerEnabled(false);
        cutscene.OnFinished += OnCutsceneFinished;
        cutscene.Play(_director);

        void OnCutsceneFinished()
        {
            cutscene.OnFinished -= OnCutsceneFinished;
            _onFinished?.Invoke();
        }
    }

    private void SetPlayerEnabled(bool _enabled)
    {
        GameEvents.RaisePlayerInputLocked(!_enabled);
    }
}
