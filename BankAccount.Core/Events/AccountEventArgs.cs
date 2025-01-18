namespace BankAccount.Core.Events;

public class AccountEventArgs(Guid accountId, string accountHolder, decimal balance) : EventArgs
{
    public Guid AccountId { get; } = accountId;
    public string AccountHolder { get; } = accountHolder;
    public decimal Balance { get; } = balance;
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}