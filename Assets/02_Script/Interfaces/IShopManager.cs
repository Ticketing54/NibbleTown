using System.Collections.Generic;

public interface IShopManager
{
    IReadOnlyList<int> CurrentEntries { get; }
}
