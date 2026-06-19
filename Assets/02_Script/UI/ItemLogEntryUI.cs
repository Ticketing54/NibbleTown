using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ItemLogEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float           fadeDuration = 0.5f;

    private Action<ItemLogEntryUI> onReturn;
    private Coroutine              activeCoroutine;

    public void Show(string _message, float _displayDuration, Action<ItemLogEntryUI> _onReturn)
    {
        onReturn = _onReturn;

        messageText.text  = _message;
        messageText.color = Color.white;

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(CoFadeAndReturn(_displayDuration));
    }

    private IEnumerator CoFadeAndReturn(float _displayDuration)
    {
        yield return new WaitForSeconds(_displayDuration);

        float elapsed = 0f;
        Color color   = messageText.color;

        while (elapsed < fadeDuration)
        {
            elapsed      += Time.deltaTime;
            color.a       = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            messageText.color = color;
            yield return null;
        }

        onReturn?.Invoke(this);
    }
}
