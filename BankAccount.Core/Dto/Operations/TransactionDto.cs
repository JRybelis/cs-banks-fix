namespace BankAccount.Core.Dto.Operations;

public record TransactionDto(
    Guid TransactionId,
    Guid AccountId,
    decimal amount,
    string Description,
    DateTime Timestamp);
