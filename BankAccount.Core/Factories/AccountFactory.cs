using BankAccount.Core.Constants;
using BankAccount.Core.Domain;
using BankAccount.Core.Domain.Enums;
using BankAccount.Core.Interfaces.Domain;

namespace BankAccountBase.Core.Factories;

public class AccountFactory
{
    private static readonly Dictionary<AccountType, Type> AccountTypes = new()
    {
        { AccountType.Checking, typeof(CheckingAccount) },
        { AccountType.Savings, typeof(SavingsAccount) }
    };

    public static IAccount CreateAccount(AccountType accountType, string accountHolder, decimal rateOrLimit)
    {
        if (!AccountTypes.TryGetValue(accountType, out var type))
            throw new ArgumentException($"Unknown account type: {accountType}");
        
        // Create account instance using reflection
        var account = (IAccount)Activator.CreateInstance(
            type,
            new object[] { accountHolder, rateOrLimit })!;
        
        var balanceChangedEvent = type.GetEvent(AccountConstants.EventNames.BalanceChanged);
        if (balanceChangedEvent == null)
            throw new InvalidOperationException($"Account type {accountType} must support the {AccountConstants.EventNames.BalanceChanged} event");
        
        return account;
    }
}