using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerDetectionArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider _col)
    {
        if (_col.TryGetComponent(out MonsterController mc))
            GameEvents.RaiseHUDShow(mc);
    }

    private void OnTriggerExit(Collider _col)
    {
        if (_col.TryGetComponent(out MonsterController mc))
            GameEvents.RaiseHUDHide(mc);
    }
}
