using System;

public interface IHasHP
{
    int    CurrentHP    { get; }
    int    MaxHP        { get; }
    bool   IsDead       { get; }

    event Action OnHit;
    event Action OnDied;
}
