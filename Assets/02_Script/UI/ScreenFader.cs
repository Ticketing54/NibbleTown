using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private float defaultDuration = 0.4f;

    private void Awake()
    {
        Color c = panel.color;
        c.a         = 0f;
        panel.color = c;
        panel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnFadeInRequested  += OnFadeInRequested;
        GameEvents.OnFadeOutRequested += OnFadeOutRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnFadeInRequested  -= OnFadeInRequested;
        GameEvents.OnFadeOutRequested -= OnFadeOutRequested;
    }

    private void OnFadeInRequested(float duration, Action onComplete) =>
        StartCoroutine(CoFade(0f, 1f, duration < 0f ? defaultDuration : duration, true, onComplete));

    private void OnFadeOutRequested(float duration, Action onComplete) =>
        StartCoroutine(CoFade(1f, 0f, duration < 0f ? defaultDuration : duration, false, onComplete));

    private IEnumerator CoFade(float from, float to, float duration, bool activate, Action onComplete)
    {
        if (activate) panel.gameObject.SetActive(true);

        float elapsed = 0f;
        Color c       = panel.color;

        while (elapsed < duration)
        {
            elapsed    += Time.deltaTime;
            c.a         = Mathf.Lerp(from, to, elapsed / duration);
            panel.color = c;
            yield return null;
        }

        c.a         = to;
        panel.color = c;

        if (!activate) panel.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
