using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    private Text   _apText;
    private Text   _levelText;
    private Text   _dayText;
    private Image  _apFill;
    private GameObject _endDayBtn;

    private void Awake()
    {
        BuildHUD();
    }

    private void Start()
    {
        Subscribe();
        if (ActionPointSystem.Instance != null)
            Refresh(ActionPointSystem.Instance.Current, ActionPointSystem.Instance.Max);
        if (DayManager.Instance != null)
            _dayText.text = "Day " + DayManager.Instance.Day;
    }

    private void OnDestroy()
    {
        if (ActionPointSystem.Instance == null) return;
        ActionPointSystem.Instance.OnAPChanged -= Refresh;
        ActionPointSystem.Instance.OnLevelUp   -= OnLevelUp;
        ActionPointSystem.Instance.OnAPLow     -= ShowEndDayButton;
        if (DayManager.Instance != null)
            DayManager.Instance.OnDayChanged   -= OnDayChanged;
    }

    private void Subscribe()
    {
        if (ActionPointSystem.Instance == null) return;
        ActionPointSystem.Instance.OnAPChanged += Refresh;
        ActionPointSystem.Instance.OnLevelUp   += OnLevelUp;
        ActionPointSystem.Instance.OnAPLow     += ShowEndDayButton;
        if (DayManager.Instance != null)
            DayManager.Instance.OnDayChanged   += OnDayChanged;
    }

    private void Refresh(int current, int max)
    {
        _apText.text       = current + " / " + max;
        _apFill.fillAmount = max > 0 ? (float)current / max : 0f;

        if (ActionPointSystem.Instance != null)
            _levelText.text = "Lv." + ActionPointSystem.Instance.Level;
    }

    private void OnLevelUp(int level)
    {
        _levelText.text = "Lv." + level;
        Debug.Log($"[Level Up!] Lv.{level} — 최대 행동력: {ActionPointSystem.Instance.Max}");
    }

    private void ShowEndDayButton()
    {
        _endDayBtn.SetActive(true);
    }

    private void OnDayChanged(int day)
    {
        _dayText.text = "Day " + day;
        _endDayBtn.SetActive(false);
    }

    // ── UI 빌드 ──────────────────────────────────────────

    private void BuildHUD()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        BuildAPPanel();
        BuildDayPanel();
        BuildEndDayButton();
    }

    private void BuildAPPanel()
    {
        var panel     = MakeGO("APPanel", transform);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin        = new Vector2(0f, 1f);
        panelRect.anchorMax        = new Vector2(0f, 1f);
        panelRect.pivot            = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(20f, -20f);
        panelRect.sizeDelta        = new Vector2(260f, 80f);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        // 레벨 텍스트
        var levelGO   = MakeGO("LevelText", panel.transform);
        var levelRect = levelGO.AddComponent<RectTransform>();
        levelRect.anchorMin        = new Vector2(0f, 1f);
        levelRect.anchorMax        = new Vector2(1f, 1f);
        levelRect.pivot            = new Vector2(0f, 1f);
        levelRect.anchoredPosition = new Vector2(10f, -8f);
        levelRect.sizeDelta        = new Vector2(-10f, 24f);
        _levelText           = SetupText(levelGO, "Lv.1", 18, FontStyle.Bold, new Color(1f, 0.85f, 0.2f), TextAnchor.MiddleLeft);

        // AP 바 배경
        var bgGO   = MakeGO("APBar_BG", panel.transform);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin        = new Vector2(0f, 0f);
        bgRect.anchorMax        = new Vector2(1f, 0f);
        bgRect.pivot            = new Vector2(0.5f, 0f);
        bgRect.anchoredPosition = new Vector2(0f, 8f);
        bgRect.sizeDelta        = new Vector2(-20f, 14f);
        bgGO.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1f);

        // AP 채움
        var fillGO   = MakeGO("APBar_Fill", bgGO.transform);
        var fillRect = fillGO.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        _apFill            = fillGO.AddComponent<Image>();
        _apFill.color      = new Color(0.2f, 0.75f, 1f, 1f);
        _apFill.type       = Image.Type.Filled;
        _apFill.fillMethod = Image.FillMethod.Horizontal;
        _apFill.fillAmount = 1f;

        // AP 숫자 텍스트
        var apTextGO   = MakeGO("APText", panel.transform);
        var apTextRect = apTextGO.AddComponent<RectTransform>();
        apTextRect.anchorMin        = new Vector2(0f, 0.3f);
        apTextRect.anchorMax        = new Vector2(1f, 0.85f);
        apTextRect.offsetMin        = new Vector2(10f, 0f);
        apTextRect.offsetMax        = new Vector2(-10f, 0f);
        _apText = SetupText(apTextGO, "10 / 10", 20, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
    }

    private void BuildDayPanel()
    {
        var panel     = MakeGO("DayPanel", transform);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin        = new Vector2(1f, 1f);
        panelRect.anchorMax        = new Vector2(1f, 1f);
        panelRect.pivot            = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-20f, -20f);
        panelRect.sizeDelta        = new Vector2(160f, 50f);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        var textGO   = MakeGO("DayText", panel.transform);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 4f);
        textRect.offsetMax = new Vector2(-8f, -4f);
        _dayText = SetupText(textGO, "Day 1", 24, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
    }

    private void BuildEndDayButton()
    {
        _endDayBtn = MakeGO("EndDayButton", transform);
        var btnRect = _endDayBtn.AddComponent<RectTransform>();
        btnRect.anchorMin        = new Vector2(0.5f, 0f);
        btnRect.anchorMax        = new Vector2(0.5f, 0f);
        btnRect.pivot            = new Vector2(0.5f, 0f);
        btnRect.anchoredPosition = new Vector2(0f, 30f);
        btnRect.sizeDelta        = new Vector2(260f, 64f);

        var bg = _endDayBtn.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.3f, 0.92f);

        var btn = _endDayBtn.AddComponent<Button>();
        btn.targetGraphic = bg;

        var colors         = btn.colors;
        colors.highlightedColor = new Color(0.2f, 0.2f, 0.55f, 0.95f);
        colors.pressedColor     = new Color(0.05f, 0.05f, 0.2f, 0.95f);
        btn.colors = colors;

        btn.onClick.AddListener(() => DayManager.Instance?.AdvanceDay());

        var textGO   = MakeGO("BtnText", _endDayBtn.transform);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        SetupText(textGO, "밤 맞이하기", 22, FontStyle.Bold, new Color(0.75f, 0.88f, 1f), TextAnchor.MiddleCenter);

        _endDayBtn.SetActive(false);
    }

    // ── 헬퍼 ────────────────────────────────────────────

    private static GameObject MakeGO(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Text SetupText(GameObject go, string text, int size, FontStyle style, Color color, TextAnchor align)
    {
        var t       = go.AddComponent<Text>();
        t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text      = text;
        t.fontSize  = size;
        t.fontStyle = style;
        t.color     = color;
        t.alignment = align;
        return t;
    }
}
