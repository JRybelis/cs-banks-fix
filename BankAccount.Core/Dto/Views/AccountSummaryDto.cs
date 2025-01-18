namespace BankAccount.Core.Dto.Views;

public record AccountSummaryDto(
    Guid AccountId,
    string AccountHolder,
    string AccountType,
    decimal Balance,
    decimal? OverdraftLimit = null,
    decimal? InterestRate = null);