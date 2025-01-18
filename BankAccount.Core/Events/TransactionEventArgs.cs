namespace BankAccount.Core.Events;

public class TransactionEventArgs(
    Guid accountId,
    string accountHolder,
    decimal balance,
    Guid transactionId,
    decimal amount,
    string transactionType)
    : AccountEventArgs(accountId, accountHolder, balance)
{
    public Guid TransactionId { get; } = transactionId;
    public decimal Amount { get; } = amount;
    public string TransactionType { get; } = transactionType;
}