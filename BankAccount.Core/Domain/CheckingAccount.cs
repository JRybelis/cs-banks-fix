using BankAccount.Core.Exceptions;
using BankAccount.Core.Interfaces.Domain;

namespace BankAccount.Core.Domain;

public class CheckingAccount : BankAccountBase
{
    private readonly decimal _overdraftLimit;
    private readonly object _checkingAccountLock = new();
    
    public decimal OverdraftLimit => _overdraftLimit;
    
    public CheckingAccount(string accountHolder, decimal overdraftLimit) : base(accountHolder)
    {
        if (overdraftLimit < 0)
            throw new ArgumentException("Overdraft limit cannot be negative", nameof(overdraftLimit));
        
        _overdraftLimit = overdraftLimit;
    }

    protected override object GetBalanceLock() => _checkingAccountLock;
    
    protected override async Task<bool> CanWithdrawAsync(decimal amount)
    {
        var currentBalance = await GetBalanceAsync();
        return currentBalance - amount >= -_overdraftLimit;
    }

    protected override async Task ProcessWithdrawalAsync(ITransaction transaction)
    {
        var currentBalance = await GetBalanceAsync();
        if (currentBalance + transaction.Amount < -_overdraftLimit) // transaction amount is already negative
            throw new InsufficientFundsException(Math.Abs(transaction.Amount), currentBalance);
        
        await SetBalanceAsync(currentBalance + transaction.Amount);
    }
}