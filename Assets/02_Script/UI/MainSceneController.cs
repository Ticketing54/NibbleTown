using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] private Image mainLogo;
    [SerializeField] private float fadeDuration = 2f;

    public void OnPlayButtonClick() => SceneLoader.LoadPlayScene();

    private void Start()
    {
        StartCoroutine(CoMainLogoAnimation());
    }

    private IEnumerator CoMainLogoAnimation()
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
