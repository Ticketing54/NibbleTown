using System;

public interface IActionPoints
{
    int Current { get; }
    int Max     { get; }
    int Level   { get; }

    bool CanSpend(int amount);
    bool TrySpend(int amount);
    void Restore();

    event Action<int, int> OnAPChanged; // current, max
    event Action<int>      OnLevelUp;   // new level
    event Action           OnAPLow;     // fell below threshold
}
