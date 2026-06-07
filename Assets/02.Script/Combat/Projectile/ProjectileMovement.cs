using UnityEngine;

public abstract class ProjectileMovement : MonoBehaviour
{
    public abstract void Init(Vector3 _direction, Transform _target = null);
}
