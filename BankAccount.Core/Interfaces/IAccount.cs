namespace BankAccount.Core.Interfaces;

public interface IAccount : IDisposable
{
    Guid AccountId { get; }
    string AccountHolder { get; }
    decimal Balance { get; }
    Task<ITransaction> DepositAsync(decimal amount, CancellationToken cancellationToken = default);
    Task<ITransaction> WithdrawAsync(decimal amount, CancellationToken cancellationToken = default);
    IEnumerable<ITransaction> GetTransactionHistory();
}