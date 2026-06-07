using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject      infoCanvas;
    [SerializeField] private Image           hpFill;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private float           heightOffset = 2f;

    private IHasHP        controller;
    private Transform     anchor;
    private Camera        mainCamera;
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Coroutine     followRoutine;

    private void Awake()
    {
        mainCamera    = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(IHasHP _controller, Transform _anchor, string _monsterName, RectTransform _canvasRect)
    {
        controller    = _controller;
        anchor        = _anchor;
        canvasRect    = _canvasRect;
        nameText.text = _monsterName;
        hpFill.fillAmount = 1f;
        infoCanvas.SetActive(false);

        controller.OnHit  += RefreshHP;
        controller.OnDied += Hide;
    }

    public void Cleanup()
    {
        if (controller == null) return;
        controller.OnHit  -= RefreshHP;
        controller.OnDied -= Hide;
        controller = null;
        anchor     = null;
    }

    public void Show()
    {
        if (controller == null || controller.IsDead) return;
        infoCanvas.SetActive(true);
        followRoutine ??= StartCoroutine(FollowRoutine());
    }

    public void Hide()
    {
        infoCanvas.SetActive(false);
        if (followRoutine != null)
        {
            StopCoroutine(followRoutine);
            followRoutine = null;
        }
    }

    private IEnumerator FollowRoutine()
    {
        while (true)
        {
            if (mainCamera == null) mainCamera = Camera.main;

            if (anchor != null && mainCamera != null)
            {
                Vector3 worldPos  = anchor.position + Vector3.up * heightOffset;
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);
                rectTransform.localPosition = localPos;
            }

            yield return null;
        }
    }

    private void RefreshHP()
    {
        if (controller.MaxHP <= 0) return;
        hpFill.fillAmount = (float)controller.CurrentHP / controller.MaxHP;
    }
}
