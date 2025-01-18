namespace BankAccount.Core.Dto.Views;

public record TransactionHistoryItemDto(
    DateTime Date,
    string Description,
    decimal Amount,
    decimal CurrentBalance);