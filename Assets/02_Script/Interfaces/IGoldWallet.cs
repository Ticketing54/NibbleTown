public interface IGoldWallet
{
    int  Gold        { get; }
    bool SpendGold(int amount);
}
