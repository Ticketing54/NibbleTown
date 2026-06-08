using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameDataSettings gameDataSettings;

    private void Awake()
    {
        GameDataManager.Init(gameDataSettings);
        UserDataManager.Init();
    }
}
