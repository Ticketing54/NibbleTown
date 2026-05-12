using UnityEngine;

public interface IMovementLock
{
    void LockMovement(bool _locked);
    void LookAt(Transform _target);
}
