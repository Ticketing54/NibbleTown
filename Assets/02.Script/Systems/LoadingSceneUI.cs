using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] private Image progressImage;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private CanvasGroup canvasGroup;

    private const string LOADING_TEXT = "Loading";
    private const string LOADING_PROGRESS_TEXT = ". . .     ";

    private float timer;
    private int index;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        progressImage.transform.Rotate(Vector3.back * Time.deltaTime * 200f);

        timer += Time.deltaTime;
        if (timer > 0.1f)
        {
            timer -= 0.1f;
            index = (index + 1) % LOADING_PROGRESS_TEXT.Length;
        }

        progressText.text = LOADING_TEXT + LOADING_PROGRESS_TEXT.Substring(0, index);
    }

    public IEnumerator Show()
    {
        gameObject.SetActive(true);
        timer = 0f;
        index = 0;
        yield return StartCoroutine(Fade(1f));
    }

    public IEnumerator Hide()
    {
        yield return StartCoroutine(Fade(0f));
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;

        canvasGroup.blocksRaycasts = targetAlpha > 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = targetAlpha > 0f;
    }
}
