using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneTrigger : MonoBehaviour
{
    [SerializeField] Image mainLogo;
    [SerializeField] float fadeDuration = 2f;

    public void OnPlayButtonClick() => SceneLoader.LoadPlayScene();

    void Start()
    {
        StartCoroutine(CoMainLogoAnimation());
    }

    IEnumerator CoMainLogoAnimation()
    {
        if (mainLogo == null) yield break;

        mainLogo.color = new Color(mainLogo.color.r, mainLogo.color.g, mainLogo.color.b, 1f);

        yield return new WaitForSeconds(3f);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mainLogo.color = new Color(mainLogo.color.r, mainLogo.color.g, mainLogo.color.b, alpha);
            yield return null;
        }

        mainLogo.color = new Color(mainLogo.color.r, mainLogo.color.g, mainLogo.color.b, 0f);
    }
}
