using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IInteractionUI
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[UIManager]");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<UIManager>();
            }
            return _instance;
        }
    }

    private GameObject _hintPanel;
    private Text _hintText;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        BuildCanvas();
    }

    private void BuildCanvas()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        BuildHintPanel();
    }

    private void BuildHintPanel()
    {
        _hintPanel = new GameObject("HintPanel");
        _hintPanel.transform.SetParent(transform, false);

        var panelRect = _hintPanel.AddComponent<RectTransform>();
        panelRect.anchorMin        = new Vector2(0.5f, 0f);
        panelRect.anchorMax        = new Vector2(0.5f, 0f);
        panelRect.pivot            = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 60f);
        panelRect.sizeDelta        = new Vector2(320f, 50f);

        var bg = _hintPanel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.65f);

        var textGO   = new GameObject("HintText");
        textGO.transform.SetParent(_hintPanel.transform, false);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 8f);
        textRect.offsetMax = new Vector2(-12f, -8f);

        _hintText           = textGO.AddComponent<Text>();
        _hintText.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _hintText.fontSize  = 22;
        _hintText.fontStyle = FontStyle.Bold;
        _hintText.alignment = TextAnchor.MiddleCenter;
        _hintText.color     = Color.white;

        _hintPanel.SetActive(false);
    }

    public void ShowHint(string text)
    {
        _hintText.text = text;
        _hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        _hintPanel.SetActive(false);
    }
}
