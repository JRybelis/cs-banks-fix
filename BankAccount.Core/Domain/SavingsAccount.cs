using BankAccount.Core.Constants;
using BankAccount.Core.Interfaces.Domain;

namespace BankAccount.Core.Domain;

public class SavingsAccount : BankAccountBase, IInterestBearing
{
    private readonly SemaphoreSlim _interestLock = new(1, 1);
    private readonly object _savingsAccountLock = new();
    
    public decimal InterestRate { get; }
    
    public SavingsAccount(string accountHolder, decimal interestRate) : base(accountHolder)
    {
        if (InterestRate < 0)
            throw new ArgumentException("Interest rate cannot be negative", nameof(interestRate));
        
        InterestRate = interestRate;
    }

    protected override object GetBalanceLock() => _savingsAccountLock;
    
    public async Task<ITransaction> CalculateAndApplyInterestAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await _interestLock.WaitAsync(cancellationToken);
        try
        {
            var currentBalance = await GetBalanceAsync();
            var  interestAmount = currentBalance * (InterestRate / 100);

            var transaction = new Transaction(interestAmount, AccountConstants.TransactionTypes.Interest);
            await ProcessDepositAsync(transaction);

            OnTransactionCompleted(transaction);
            return transaction;
        }
        finally
        {
            _interestLock.Release();
        }
    }
    
    protected override async Task<bool> CanWithdrawAsync(decimal amount)
    {
        var currentBalance = await GetBalanceAsync();
        return currentBalance >= amount;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _interestLock.Dispose();
        }
        base.Dispose(disposing);
    }
}