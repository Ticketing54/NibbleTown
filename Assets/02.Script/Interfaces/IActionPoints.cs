using System;

public interface IActionPoints
{
    int Current { get; }
    int Max     { get; }
    int Level   { get; }

    bool CanSpend(int _amount);
    bool TrySpend(int _amount);
    void Restore();
}
