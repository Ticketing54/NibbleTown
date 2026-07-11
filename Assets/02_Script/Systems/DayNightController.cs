using System;
using System.Collections;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("Sun")]
    [SerializeField] private Light   sunLight;
    [SerializeField] private Vector3 dayRotationEuler   = new Vector3(90f, 253.7f, 0f);
    [SerializeField] private Vector3 nightRotationEuler = new Vector3(270f, 253.7f, 0f);
    [SerializeField] private Color   dayColor      = Color.white;
    [SerializeField] private Color   nightColor    = new Color(0.04f, 0.07f, 0.55f);
    [SerializeField] private float   dayIntensity   = 1f;
    [SerializeField] private float   nightIntensity = 0.05f;
    [SerializeField] private float   transitionDuration = 3f;

    [Header("Sky (선택)")]
    [SerializeField] private Material skyMaterial;
    [SerializeField] private string   blendProperty = "_DayNightBlend";

    public event Action OnFinished;

    private Coroutine tweenCoroutine;

    public void PlayToNight() => StartTween(true);
    public void PlayToDay()   => StartTween(false);

    private void StartTween(bool _toNight)
    {
        if (tweenCoroutine != null) StopCoroutine(tweenCoroutine);
        tweenCoroutine = StartCoroutine(CoTween(_toNight));
    }

    private IEnumerator CoTween(bool _toNight)
    {
        Transform sunTransform  = sunLight.transform;
        Quaternion fromRotation = sunTransform.rotation;
        Quaternion toRotation   = Quaternion.Euler(_toNight ? nightRotationEuler : dayRotationEuler);
        Color      fromColor    = sunLight.color;
        Color      toColor      = _toNight ? nightColor : dayColor;
        float      fromIntensity = sunLight.intensity;
        float      toIntensity   = _toNight ? nightIntensity : dayIntensity;
        float      fromBlend    = _toNight ? 0f : 1f;
        float      toBlend      = _toNight ? 1f : 0f;

        void Apply(float _t)
        {
            sunTransform.rotation = Quaternion.Slerp(fromRotation, toRotation, _t);
            sunLight.color        = Color.Lerp(fromColor, toColor, _t);
            sunLight.intensity    = Mathf.Lerp(fromIntensity, toIntensity, _t);
            if (skyMaterial != null) skyMaterial.SetFloat(blendProperty, Mathf.Lerp(fromBlend, toBlend, _t));
        }

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            Apply(Mathf.Clamp01(elapsed / transitionDuration));
            yield return null;
        }

        tweenCoroutine = null;
        OnFinished?.Invoke();
    }
}
