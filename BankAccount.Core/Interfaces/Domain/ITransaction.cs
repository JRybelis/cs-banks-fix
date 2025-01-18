namespace BankAccount.Core.Interfaces.Domain;

public interface ITransaction : IDisposable
{
    Guid TransactionId { get; }
    decimal Amount { get; }
    DateTime Timestamp { get; }
    string Description { get; }
}