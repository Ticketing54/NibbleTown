using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float           duration    = 1f;
    [SerializeField] private float           floatHeight = 1.5f;

    private Camera  mainCamera;
    private Color   originalColor;
    private Vector3 originalScale;

    private void Awake()
    {
        mainCamera    = Camera.main;
        originalColor = text.color;
        originalScale = transform.localScale;
    }

    public void Play(int _amount, Vector3 _position, bool _isCrit, Action _onComplete)
    {
        transform.position   = _position;
        transform.localScale = Vector3.zero;
        text.text            = _amount.ToString();
        text.color           = _isCrit ? Color.red : originalColor;

        StartCoroutine(AnimateRoutine(_isCrit, _onComplete));
    }

    private IEnumerator AnimateRoutine(bool _isCrit, Action _onComplete)
    {
        Vector3 startPos      = transform.position;
        Vector3 endPos        = startPos + Vector3.up * floatHeight;
        float   punchDuration = duration * 0.3f;

        // 크기 팝 (0 → originalScale * 1.3)
        yield return ScaleRoutine(Vector3.zero, originalScale * 1.3f, punchDuration * 0.5f);

        // 크기 복구 (originalScale * 1.3 → originalScale)
        yield return ScaleRoutine(originalScale * 1.3f, originalScale, punchDuration * 0.5f);

        // 위로 이동 + 페이드 아웃
        float elapsed      = 0f;
        float moveDuration = duration - punchDuration;
        Color baseColor    = _isCrit ? Color.red : originalColor;
        Color fadeColor    = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            text.color         = Color.Lerp(baseColor, fadeColor, t);
            FaceCamera();
            yield return null;
        }

        _onComplete?.Invoke();
    }

    private IEnumerator ScaleRoutine(Vector3 _from, Vector3 _to, float _time)
    {
        float elapsed = 0f;
        while (elapsed < _time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _time);
            transform.localScale = Vector3.Lerp(_from, _to, t);
            FaceCamera();
            yield return null;
        }
    }

    private void FaceCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
            transform.forward = mainCamera.transform.forward;
    }
}
