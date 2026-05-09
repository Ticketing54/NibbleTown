using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HoldProgressBar : MonoBehaviour, IProgressBar
{
    [SerializeField] private Image fill;
    private Coroutine fill_routine;
    private float progress = 0;

    private void OnEnable()
    {
        UIEvents.OnProgressBarShow        += Show;
        UIEvents.OnProgressBarSetProgress += SetProgress;
    }

    private void OnDisable()
    {
        UIEvents.OnProgressBarShow        -= Show;
        UIEvents.OnProgressBarSetProgress -= SetProgress;
    }

    public void SetProgress(float _t) => progress = _t;

    public void Show(bool _visible)
    {
        if(fill_routine != null)
        StopCoroutine(fill_routine);
        fill_routine = StartCoroutine(CoShowProgressBar());


        gameObject.SetActive(_visible);
        if (_visible) fill.fillAmount = 0f;
    }

    private IEnumerator CoShowProgressBar()
    {
        while(true)
        {
            yield return null;
            fill.fillAmount = progress;
        }
        
    }

}
