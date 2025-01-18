using BankAccount.Core.Events;


namespace BankAccount.Core.Interfaces.Events;

public interface IAccountEventPublisher
{
    event EventHandler<TransactionEventArgs> TransactionCompleted;
    event EventHandler<AccountEventArgs> AccountClosed;
    event EventHandler<AccountEventArgs> BalanceChanged; 
}