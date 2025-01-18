using System.Collections.Concurrent;
using BankAccount.Core.Constants;
using BankAccount.Core.Events;
using BankAccount.Core.Exceptions;
using BankAccount.Core.Extensions;
using BankAccount.Core.Interfaces.Domain;
using BankAccount.Core.Interfaces.Events;

namespace BankAccount.Core.Domain;

public class BankAccountBase : IAccount, IAccountEventPublisher
{
    protected readonly object BalanceLock = new();
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private readonly ConcurrentQueue<ITransaction> _transactionHistory;

    public event EventHandler<TransactionEventArgs>? TransactionCompleted;
    public event EventHandler<AccountEventArgs>? AccountClosed;
    public event EventHandler<AccountEventArgs>? BalanceChanged;
    private bool _disposed;
    protected decimal balance;
    
    public Guid AccountId { get; }
    public string AccountHolder { get; }
    public decimal Balance
    {
        get
        {
            ThrowIfDisposed();
            lock (BalanceLock)
            {
                return balance;
            }
        }
    }

    protected BankAccountBase(string accountHolder)
    {
        AccountHolder = accountHolder ?? throw new ArgumentNullException(nameof(accountHolder));
        AccountId = Guid.NewGuid();
        _transactionHistory = new ConcurrentQueue<ITransaction>();
    }
    
    public virtual async Task<ITransaction> DepositAsync(decimal amount, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (!amount.IsValidTransactionAmount())
            throw new InvalidTransactionException(AccountConstants.ValidationMessages.InvalidAmount);
        
        await _asyncLock.WaitAsync(cancellationToken);
        try
        {
            var transaction = new Transaction(amount, AccountConstants.TransactionTypes.Deposit);
            await ProcessDepositAsync(transaction);
            _transactionHistory.Enqueue(transaction);
            
            // Raise events
            OnTransactionCompleted(transaction);
            OnBalanceChanged();
            return transaction;
        }
        finally
        {
            _asyncLock.Release();   
        }
    }

    protected void OnBalanceChanged()
    {
        BalanceChanged?.Invoke(this, new AccountEventArgs(
            AccountId,
            AccountHolder,
            Balance));
    }

    protected void OnTransactionCompleted(Transaction transaction)
    {
        TransactionCompleted?.Invoke(this, new TransactionEventArgs(
            AccountId,
            AccountHolder,
            Balance,
            transaction.TransactionId,
            transaction.Amount,
            transaction.Description));
    }

    public virtual async Task<ITransaction> WithdrawAsync(decimal amount, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (amount <= 0) throw new InvalidTransactionException("Amount cannot be zero or negative.");
        
        await _asyncLock.WaitAsync(cancellationToken);
        try
        {
            if (!await CanWithdrawAsync(amount))
                throw new InsufficientFundsException(amount, Balance);
            
            var transaction = new Transaction(-amount, AccountConstants.TransactionTypes.Withdrawal);
            await ProcessWithdrawalAsync(transaction);
            _transactionHistory.Enqueue(transaction);
            return transaction;
        }
        finally
        {
            _asyncLock.Release();
        }
    }

    public IEnumerable<ITransaction> GetTransactionHistory()
    {
        ThrowIfDisposed();
        return _transactionHistory.ToArray();
    }

    protected virtual async Task ProcessDepositAsync(ITransaction transaction)
    {
        await Task.Run(() =>
        {
            lock (BalanceLock)
            {
                balance += transaction.Amount;
            }
        });
    }

    protected virtual async Task ProcessWithdrawalAsync(ITransaction transaction)
    {
        await Task.Run(() =>
        {
            lock (BalanceLock)
            {
                balance += transaction.Amount; // Amount is already negative
            }
        });
    }

    protected virtual async Task<bool> CanWithdrawAsync(decimal amount)
    {
        return await Task.Run(() =>
        {
            lock (BalanceLock)
            {
                return balance >= amount;
            }
        });
    }
    
    public void Dispose()
    {
        // Don't dispose more than once
        if (_disposed) return;
        
        // Call the virtual dispose method
        Dispose(true);
        
        // Suppress finalization since we have disposed
        GC.SuppressFinalize(this);
        
        _disposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        
        _asyncLock.Dispose();
        foreach (var transaction in _transactionHistory)
        {
            transaction.Dispose();
        }
            
        // Raise the AccountClosed event before disposing
        AccountClosed?.Invoke(this, new AccountEventArgs(AccountId, AccountHolder, Balance));
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, GetType().Name);
    }
}