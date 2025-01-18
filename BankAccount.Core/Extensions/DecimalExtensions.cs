using BankAccount.Core.Constants;

namespace BankAccount.Core.Extensions;

public static class DecimalExtensions
{
    public static bool IsWithinRange(this decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    public static bool IsValidTransactionAmount(this decimal amount)
    {
        return amount.IsWithinRange(AccountConstants.Limits.MinDeposit, AccountConstants.Limits.MaxDeposit);
    }

    public static bool IsValidBalance(this decimal balance)
    {
        return balance.IsWithinRange(AccountConstants.Limits.MinBalance, AccountConstants.Limits.MaxBalance);
    }
}