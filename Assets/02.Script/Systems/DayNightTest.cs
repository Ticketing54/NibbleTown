using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 낮↔밤 전환 테스트. [N] 낮→밤  [D] 밤→낮
/// 테스트 완료 후 제거 예정.
/// </summary>
public class DayNightTest : MonoBehaviour
{
    [SerializeField] private Light  directionalLight;
    [SerializeField] private float  transitionDuration = 3f;

    [Header("해질녘 (낮 → 밤)")]
    [SerializeField] private Gradient sunsetAmbient;
    [SerializeField] private Gradient sunsetLightColor;
    [SerializeField] private AnimationCurve sunsetIntensity = AnimationCurve.Linear(0f, 1f, 1f, 0.05f);

    [Header("동틀녘 (밤 → 낮)")]
    [SerializeField] private Gradient sunriseAmbient;
    [SerializeField] private Gradient sunriseLightColor;
    [SerializeField] private AnimationCurve sunriseIntensity = AnimationCurve.Linear(0f, 0.05f, 1f, 1f);

    private bool transitioning;

    private void Awake()
    {
        SetDefaultGradients();
    }

    private void Update()
    {
        if (transitioning) return;

        if (Keyboard.current.nKey.wasPressedThisFrame)
            StartCoroutine(CoToNight());
        else if (Keyboard.current.dKey.wasPressedThisFrame)
            StartCoroutine(CoToDay());
    }

    private IEnumerator CoToNight()
    {
        transitioning = true;
        yield return StartCoroutine(CoTransition(sunsetAmbient, sunsetLightColor, sunsetIntensity));
        GameEvents.RaiseNightBegin();
        transitioning = false;
    }

    private IEnumerator CoToDay()
    {
        transitioning = true;
        yield return StartCoroutine(CoTransition(sunriseAmbient, sunriseLightColor, sunriseIntensity));
        GameEvents.RaiseDayBegin();
        transitioning = false;
    }

    private IEnumerator CoTransition(Gradient _ambient, Gradient _lightColor, AnimationCurve _intensity)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            RenderSettings.ambientLight = _ambient.Evaluate(smooth);

            if (directionalLight != null)
            {
                directionalLight.color     = _lightColor.Evaluate(smooth);
                directionalLight.intensity = _intensity.Evaluate(smooth);
            }

            yield return null;
        }
    }

    private void SetDefaultGradients()
    {
        // 해질녘: 낮(흰) → 주황 → 붉은 → 밤(짙은 파랑)
        sunsetAmbient = new Gradient();
        sunsetAmbient.SetKeys(
            new[] {
                new GradientColorKey(new Color(0.5f, 0.5f, 0.5f),   0.0f),
                new GradientColorKey(new Color(0.4f, 0.2f, 0.05f),  0.5f),
                new GradientColorKey(new Color(0.03f, 0.03f, 0.08f), 1.0f),
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        sunsetLightColor = new Gradient();
        sunsetLightColor.SetKeys(
            new[] {
                new GradientColorKey(Color.white,                    0.0f),
                new GradientColorKey(new Color(1f, 0.45f, 0.1f),    0.45f),
                new GradientColorKey(new Color(0.6f, 0.1f, 0.05f),  0.65f),
                new GradientColorKey(new Color(0.1f, 0.1f, 0.3f),   1.0f),
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        // 동틀녘: 밤(짙은 파랑) → 붉은 → 주황 → 낮(흰)
        sunriseAmbient = new Gradient();
        sunriseAmbient.SetKeys(
            new[] {
                new GradientColorKey(new Color(0.03f, 0.03f, 0.08f), 0.0f),
                new GradientColorKey(new Color(0.3f, 0.1f, 0.05f),  0.4f),
                new GradientColorKey(new Color(0.5f, 0.5f, 0.5f),   1.0f),
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        sunriseLightColor = new Gradient();
        sunriseLightColor.SetKeys(
            new[] {
                new GradientColorKey(new Color(0.1f, 0.1f, 0.3f),   0.0f),
                new GradientColorKey(new Color(0.7f, 0.15f, 0.05f), 0.35f),
                new GradientColorKey(new Color(1f, 0.55f, 0.15f),   0.55f),
                new GradientColorKey(Color.white,                    1.0f),
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
    }
}
