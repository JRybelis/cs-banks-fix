using System.Collections.Concurrent;
using BankAccount.Core.Constants;
using BankAccount.Core.Events;
using BankAccount.Core.Exceptions;
using BankAccount.Core.Extensions;
using BankAccount.Core.Interfaces.Domain;
using BankAccount.Core.Interfaces.Events;

namespace BankAccount.Core.Domain;

public abstract class BankAccountBase : IAccount, IAccountEventPublisher
{
    private bool _disposed;
    private decimal _balance;
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private readonly ConcurrentQueue<ITransaction> _transactionHistory;

    public event EventHandler<TransactionEventArgs>? TransactionCompleted;
    public event EventHandler<AccountEventArgs>? AccountClosed;
    public event EventHandler<AccountEventArgs>? BalanceChanged;
    
    public Guid AccountId { get; }
    public string AccountHolder { get; }
    public decimal Balance
    {
        get
        {
            ThrowIfDisposed();
            lock (GetBalanceLock())
            {
                return _balance;
            }
        }
        private set
        {
            lock (GetBalanceLock())
            {
                if (_balance == value) return;
                _balance = value;
                OnBalanceChanged();
            }
        }
    }

    protected BankAccountBase(string accountHolder)
    {
        AccountHolder = accountHolder ?? throw new ArgumentNullException(nameof(accountHolder));
        AccountId = Guid.NewGuid();
        _transactionHistory = new ConcurrentQueue<ITransaction>();
    }
    
    protected abstract object GetBalanceLock();

    protected async Task<decimal> GetBalanceAsync()
    {
        ThrowIfDisposed();
        return await Task.Run(() =>
        {
            lock (GetBalanceLock())
            {
                return _balance;
            }
        });
    }
    
    protected async Task SetBalanceAsync(decimal newBalance)
    {
        ThrowIfDisposed();
        await Task.Run(() =>
        {
            lock (GetBalanceLock())
            {
                if (_balance == newBalance) return;
                _balance = newBalance;
                OnBalanceChanged();
            }
        });
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
    
    public virtual async Task<ITransaction> WithdrawAsync(decimal amount, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (amount <= 0) throw new InvalidTransactionException(AccountConstants.ValidationMessages.InvalidAmount);
        
        await _asyncLock.WaitAsync(cancellationToken);
        try
        {
            if (!await CanWithdrawAsync(amount))
                throw new InsufficientFundsException(amount, await GetBalanceAsync());
            
            var transaction = new Transaction(-amount, AccountConstants.TransactionTypes.Withdrawal);
            await ProcessWithdrawalAsync(transaction);
            _transactionHistory.Enqueue(transaction);
            
            OnTransactionCompleted(transaction);
            return transaction;
        }
        finally
        {
            _asyncLock.Release();
        }
    }
    
    protected virtual async Task ProcessDepositAsync(ITransaction transaction)
    {
        var currentBalance = await GetBalanceAsync();
        await SetBalanceAsync(currentBalance + transaction.Amount);
    }

    protected virtual async Task ProcessWithdrawalAsync(ITransaction transaction)
    {
        var currentBalance = await GetBalanceAsync();
        await SetBalanceAsync(currentBalance + transaction.Amount); // transaction.Amount is already negative
    }
    
    protected virtual async Task<bool> CanWithdrawAsync(decimal amount)
    {
        var currentBalance = await GetBalanceAsync();
        return currentBalance >= amount;
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

    public IEnumerable<ITransaction> GetTransactionHistory()
    {
        ThrowIfDisposed();
        return _transactionHistory.ToArray();
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