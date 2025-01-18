using BankAccount.Core.Interfaces.Domain;

namespace BankAccount.Core.Domain;

public class Transaction(decimal amount, string description) : ITransaction
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public decimal Amount { get; } = amount;
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Description { get; } = description ?? throw new ArgumentNullException(nameof(description));
    private bool disposed;

    public void Dispose()
    {
        if (disposed) return;
        GC.SuppressFinalize(this);
        disposed = true;
    }
}