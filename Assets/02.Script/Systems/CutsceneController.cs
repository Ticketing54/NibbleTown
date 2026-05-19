using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private InputActionReference skipAction;

    public event Action OnFinished;

    private PlayableDirector activeDirector;
    private bool skipped;

    public void Play(PlayableDirector _director)
    {
        activeDirector = _director;
        skipped = false;

        activeDirector.stopped += OnDirectorStopped;
        activeDirector.Play();

        if (skipAction != null)
        {
            skipAction.action.Enable();
            skipAction.action.performed += OnSkipInput;
        }
    }

    public void Skip()
    {
        if (skipped || activeDirector == null) return;
        skipped = true;
        activeDirector.Stop();
    }

    private void OnSkipInput(InputAction.CallbackContext _ctx) => Skip();

    private void OnDirectorStopped(PlayableDirector _director)
    {
        activeDirector.stopped -= OnDirectorStopped;
        activeDirector = null;

        if (skipAction != null)
            skipAction.action.performed -= OnSkipInput;

        OnFinished?.Invoke();
    }
}
