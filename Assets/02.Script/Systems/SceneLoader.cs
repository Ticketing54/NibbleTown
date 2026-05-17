using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] LoadingSceneUI loadingUI;
    [SerializeField] float minLoadingTime = 1.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        loadingUI.gameObject.SetActive(false);
    }

    public static void LoadPlayScene() => Instance.StartCoroutine(Instance.CoLoad("PlayScene"));
    public static void LoadMainScene() => Instance.StartCoroutine(Instance.CoLoad("MainScene"));

    IEnumerator CoLoad(string targetScene)
    {
        yield return StartCoroutine(loadingUI.Show());

        var op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        while (op.progress < 0.9f || elapsed < minLoadingTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;

        yield return new WaitUntil(() => op.isDone);

        yield return StartCoroutine(loadingUI.Hide());
    }
}
