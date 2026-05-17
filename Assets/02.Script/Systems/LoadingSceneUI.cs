using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] Image progressImage;
    [SerializeField] TMP_Text progressText;    
    [SerializeField] float fadeDuration = 0.4f;
    [SerializeField] CanvasGroup canvasGroup;

    const string LOADING_TEXT = "Loading";
    const string LOADINGProgress_TEXT = ". . .     ";

    float timer;
    int index;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        progressImage.transform.Rotate(Vector3.back * Time.deltaTime * 200f);

        timer += Time.deltaTime;
        if (timer > 0.1f)
        {
            timer -= 0.1f;
            index = (index + 1) % LOADING_TEXT.Length;
        }

        progressText.text = LOADING_TEXT + LOADINGProgress_TEXT.Substring(0, index);
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

    IEnumerator Fade(float targetAlpha)
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
