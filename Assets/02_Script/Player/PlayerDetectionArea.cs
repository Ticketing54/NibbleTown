using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerDetectionArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider _col)
    {
        if (_col.TryGetComponent(out MonsterController mc))
            GameHUDManager.Instance?.ShowHUD(mc);
    }

    private void OnTriggerExit(Collider _col)
    {
        if (_col.TryGetComponent(out MonsterController mc))
            GameHUDManager.Instance?.HideHUD(mc);
    }
}
