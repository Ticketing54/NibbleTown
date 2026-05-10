using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI apText;
    [SerializeField] private TextMeshProUGUI   levelText;
    [SerializeField] private TextMeshProUGUI   dayText;
    [SerializeField] private Image  apFill;
    [SerializeField] private GameObject endDayBtn;

    private void Awake()
    {
        BuildHUD();
    }

    private void OnEnable()
    {
        GameEvents.OnAPChanged            += Refresh;
        GameEvents.OnLevelUp              += OnLevelUp;
        GameEvents.OnAPNotEnough           += ShowEndDayButton;
        GameEvents.OnDayChanged           += OnDayChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnAPChanged            -= Refresh;
        GameEvents.OnLevelUp              -= OnLevelUp;
        GameEvents.OnAPNotEnough           -= ShowEndDayButton;
        GameEvents.OnDayChanged           -= OnDayChanged;
    }

    private void Refresh(int _current, int _max)
    {
        apText.text       = _current + " / " + _max;
        apFill.fillAmount = _max > 0 ? (float)_current / _max : 0f;

        if (ActionPointSystem.Instance != null)
            levelText.text = "Lv." + ActionPointSystem.Instance.Level;
    }

    private void OnLevelUp(int _level)
    {
        levelText.text = "Lv." + _level;
        Debug.Log($"[Level Up!] Lv.{_level} — 최대 행동력: {ActionPointSystem.Instance.Max}");
    }

    private void ShowEndDayButton()
    {
        endDayBtn.SetActive(true);
    }

    private void OnDayChanged(int _day)
    {
        dayText.text = "Day " + _day;
        endDayBtn.SetActive(false);
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
        BuildEndDayButton();
    }

    private void BuildEndDayButton()
    {
        endDayBtn = MakeGO("EndDayButton", transform);
        var btnRect = endDayBtn.AddComponent<RectTransform>();
        btnRect.anchorMin        = new Vector2(0.5f, 0f);
        btnRect.anchorMax        = new Vector2(0.5f, 0f);
        btnRect.pivot            = new Vector2(0.5f, 0f);
        btnRect.anchoredPosition = new Vector2(0f, 30f);
        btnRect.sizeDelta        = new Vector2(260f, 64f);

        var bg = endDayBtn.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.3f, 0.92f);

        var btn = endDayBtn.AddComponent<Button>();
        btn.targetGraphic = bg;

        var colors = btn.colors;
        colors.highlightedColor = new Color(0.2f, 0.2f, 0.55f, 0.95f);
        colors.pressedColor     = new Color(0.05f, 0.05f, 0.2f, 0.95f);
        btn.colors = colors;

        btn.onClick.AddListener(() => DayManager.Instance?.AdvanceDay());

        var textGO   = MakeGO("BtnText", endDayBtn.transform);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        SetupText(textGO, "밤 맞이하기", 22, FontStyle.Bold, new Color(0.75f, 0.88f, 1f), TextAnchor.MiddleCenter);

        endDayBtn.SetActive(false);
    }

    // ── 헬퍼 ────────────────────────────────────────────

    private static GameObject MakeGO(string _name, Transform _parent)
    {
        var go = new GameObject(_name);
        go.transform.SetParent(_parent, false);
        return go;
    }

    private static Text SetupText(GameObject _go, string _text, int _size, FontStyle _style, Color _color, TextAnchor _align)
    {
        var t       = _go.AddComponent<Text>();
        t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text      = _text;
        t.fontSize  = _size;
        t.fontStyle = _style;
        t.color     = _color;
        t.alignment = _align;
        return t;
    }
}
