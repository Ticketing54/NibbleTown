using System;

public interface IActionPoints
{
    int Current { get; }
    int Max     { get; }

    bool CanSpend(int _amount);
    bool TrySpend(int _amount);
    void Restore();
}
