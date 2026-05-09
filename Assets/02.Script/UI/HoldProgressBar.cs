using UnityEngine;
using UnityEngine.UI;

public class HoldProgressBar : MonoBehaviour, IProgressBar
{
    [SerializeField] private Color fillColor = new Color(0.3f, 0.85f, 0.4f, 1f);
    [SerializeField] private Vector2 screenAnchoredPos = new Vector2(0f, 120f);
    [SerializeField] private Vector2 barSize = new Vector2(320f, 12f);

    private GameObject _root;
    private Image _fill;

    private void Awake()
    {
        BuildBar();
        Show(false);
    }

    private void BuildBar()
    {
        var canvas = new GameObject("[HoldProgressBar]").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 101;
        canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        ((CanvasScaler)canvas.gameObject.GetComponent<CanvasScaler>()).referenceResolution = new Vector2(1920, 1080);
        DontDestroyOnLoad(canvas.gameObject);

        _root = new GameObject("Bar");
        _root.transform.SetParent(canvas.transform, false);

        var rootRect = _root.AddComponent<RectTransform>();
        rootRect.anchorMin        = new Vector2(0.5f, 0f);
        rootRect.anchorMax        = new Vector2(0.5f, 0f);
        rootRect.pivot            = new Vector2(0.5f, 0f);
        rootRect.anchoredPosition = screenAnchoredPos;
        rootRect.sizeDelta        = barSize;

        var bg = _root.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.15f);

        var fillGO   = new GameObject("Fill");
        fillGO.transform.SetParent(_root.transform, false);
        var fillRect = fillGO.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        _fill            = fillGO.AddComponent<Image>();
        _fill.color      = fillColor;
        _fill.type       = Image.Type.Filled;
        _fill.fillMethod = Image.FillMethod.Horizontal;
        _fill.fillAmount = 0f;
    }

    public void SetProgress(float t)
    {
        _fill.fillAmount = Mathf.Clamp01(t);
    }

    public void Show(bool visible)
    {
        if (_root != null) _root.SetActive(visible);
        if (visible) _fill.fillAmount = 0f;
    }
}
