using BankAccount.Core.Constants;
using BankAccount.Core.Interfaces.Domain;

namespace BankAccount.Core.Domain;

public class SavingsAccount : BankAccountBase, IInterestBearing
{
    private readonly SemaphoreSlim _interestLock = new SemaphoreSlim(1, 1);
    
    public decimal InterestRate { get; }
    
    public SavingsAccount(string accountHolder, decimal interestRate) : base(accountHolder)
    {
        if (InterestRate < 0)
            throw new ArgumentException("Interest rate cannot be negative", nameof(interestRate));
        
        InterestRate = interestRate;
    }

    public async Task<ITransaction> CalculateAndApplyInterestAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await _interestLock.WaitAsync(cancellationToken);
        try
        {
            decimal interestAmount;
            lock (BalanceLock)
            {
                interestAmount = balance * (InterestRate / 100);
            }

            var transaction = new Transaction(interestAmount, AccountConstants.TransactionTypes.Interest);
            await ProcessDepositAsync(transaction);

            return transaction;
        }
        finally
        {
            _interestLock.Release();
        }
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