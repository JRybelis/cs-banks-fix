namespace BankAccount.Core.Interfaces;

public interface ITransaction : IDisposable
{
    Guid TransactionId { get; }
    decimal Amount { get; }
    DateTime TimeStamp { get; }
    string Description { get; }
}