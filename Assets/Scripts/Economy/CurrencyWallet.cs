using System;

// Runtime state; persistence and transaction coordination belong to later phases.
public sealed class CurrencyWallet
{
    public const int DefaultStartingBalance = 100;
    public int Balance { get; private set; }
    public event Action<int> BalanceChanged;

    public CurrencyWallet(int startingBalance = DefaultStartingBalance)
    {
        if (startingBalance < 0) throw new ArgumentOutOfRangeException(nameof(startingBalance));
        Balance = startingBalance;
    }

    public bool TryAdd(int amount)
    {
        if (amount <= 0 || amount > int.MaxValue - Balance) return false;
        Balance += amount;
        BalanceChanged?.Invoke(Balance);
        return true;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || amount > Balance) return false;
        Balance -= amount;
        BalanceChanged?.Invoke(Balance);
        return true;
    }
}
