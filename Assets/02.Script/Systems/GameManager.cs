using UnityEngine;

public class GameManager : MonoBehaviour
{
    static bool initialized;

    private void Awake()
    {
        if (initialized)
        {
            Destroy(gameObject);
            return;
        }

        initialized = true;
        DontDestroyOnLoad(gameObject);
        GameDataManager.Init();
    }
}
