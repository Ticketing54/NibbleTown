using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject holdBundle;
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI interactionText;

    private Coroutine progressRoutine;
    private float progress;

    private void OnEnable()
    {
        GameEvents.OnProgressBarShow        += ShowProgressBar;
        GameEvents.OnProgressBarSetProgress += SetProgress;
        GameEvents.OnInteractionTextShow    += ShowInteractionText;
    }

    private void OnDisable()
    {
        GameEvents.OnProgressBarShow        -= ShowProgressBar;
        GameEvents.OnProgressBarSetProgress -= SetProgress;
        GameEvents.OnInteractionTextShow    -= ShowInteractionText;
    }

    public void SetProgress(float _t) => progress = _t;

    public void ShowInteractionText(bool _show,string _text)
    {
        if (_show) interactionText.text = _text;
        interactionText.gameObject.SetActive(_show);
    }

    public void ShowProgressBar(bool _visible)
    {
        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
            progressRoutine = null;
        }

        holdBundle.SetActive(_visible);

        if (_visible)
        {
            fill.fillAmount = 0f;
            progressRoutine = StartCoroutine(CoFillProgress());
        }
    }

    private IEnumerator CoFillProgress()
    {
        while (true)
        {
            yield return null;
            fill.fillAmount = progress;
        }
    }
}
