using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float           duration    = 1f;
    [SerializeField] private float           floatHeight = 80f;

    private Camera        mainCamera;
    private Color         originalColor;
    private Vector3       originalScale;
    private RectTransform rectTransform;

    private void Awake()
    {
        mainCamera    = Camera.main;
        originalColor = text.color;
        originalScale = transform.localScale;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Play(int _amount, Vector3 _worldPosition, bool _isCrit, RectTransform _canvasRect, Action _onComplete)
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, _worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, null, out Vector2 localPos);
        rectTransform.localPosition = localPos;

        transform.localScale = Vector3.zero;
        text.text            = _amount.ToString();
        text.color           = _isCrit ? Color.red : originalColor;

        StartCoroutine(AnimateRoutine(_isCrit, _onComplete));
    }

    private IEnumerator AnimateRoutine(bool _isCrit, Action _onComplete)
    {
        Vector2 startPos      = rectTransform.localPosition;
        Vector2 endPos        = startPos + Vector2.up * floatHeight;
        float   punchDuration = duration * 0.3f;

        yield return ScaleRoutine(Vector3.zero, originalScale * 1.3f, punchDuration * 0.5f);
        yield return ScaleRoutine(originalScale * 1.3f, originalScale, punchDuration * 0.5f);

        float elapsed      = 0f;
        float moveDuration = duration - punchDuration;
        Color baseColor    = _isCrit ? Color.red : originalColor;
        Color fadeColor    = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            rectTransform.localPosition = Vector2.Lerp(startPos, endPos, t);
            text.color                  = Color.Lerp(baseColor, fadeColor, t);
            yield return null;
        }

        text.color = originalColor;
        _onComplete?.Invoke();
    }

    private IEnumerator ScaleRoutine(Vector3 _from, Vector3 _to, float _time)
    {
        float elapsed = 0f;
        while (elapsed < _time)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_from, _to, Mathf.Clamp01(elapsed / _time));
            yield return null;
        }
    }
}
