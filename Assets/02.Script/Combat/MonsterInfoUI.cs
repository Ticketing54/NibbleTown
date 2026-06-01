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

    private IHasHP    controller;
    private Transform anchor;
    private Camera    mainCamera;
    private Coroutine billboardRoutine;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Init(IHasHP _controller, Transform _anchor, string _monsterName)
    {
        controller        = _controller;
        anchor            = _anchor;
        nameText.text     = _monsterName;
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
        billboardRoutine ??= StartCoroutine(BillboardRoutine());
    }

    public void Hide()
    {
        infoCanvas.SetActive(false);
        if (billboardRoutine != null)
        {
            StopCoroutine(billboardRoutine);
            billboardRoutine = null;
        }
    }

    private IEnumerator BillboardRoutine()
    {
        while (true)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (anchor != null)
                transform.position = anchor.position + Vector3.up * heightOffset;

            if (mainCamera != null)
                infoCanvas.transform.forward = mainCamera.transform.forward;

            yield return null;
        }
    }

    private void RefreshHP()
    {
        if (controller.MaxHP <= 0) return;
        hpFill.fillAmount = (float)controller.CurrentHP / controller.MaxHP;
    }
}
