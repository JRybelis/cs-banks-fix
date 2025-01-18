namespace BankAccount.Core.Dto.Views;

public record AccountBalanceDto(
    Guid AccountId,
    decimal Balance,
    decimal AvailableBalance,
    DateTime LastUpdated);
